using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using Npgsql;

namespace CleanDDDArchitecture.Domains.Account.Tests.Integration.Infrastructure;

internal sealed partial class AccountIntegrationEnvironment : IAsyncDisposable
{
    private const string DatabaseName = "cleandddarchitecture";
    private const string DatabaseUser = "postgres";
    private const string DatabasePassword = "postgres";
    private const string JwtIssuer = "cleandddarchitecture";
    private const string JwtAudience = "cleandddarchitecture-clients";
    private const string JwtAccessKey256 = "0123456789abcdef0123456789abcdef";
    private const string JwtAccessKey512 = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";
    private const string JwtRefreshKey256 = "fedcba9876543210fedcba9876543210";
    private const string JwtRefreshKey512 = "fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210";
    private const string KafkaTopic = "CleanDDDArchitectureEvents-AccountAggregate";

    private readonly string _repositoryRoot;
    private readonly string _artifactsRoot;
    private readonly string _startupLogPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _mailpitClient = new() { Timeout = TimeSpan.FromSeconds(10) };
    private readonly HttpClient _eventStoreClient = new() { Timeout = TimeSpan.FromSeconds(10) };
    private readonly List<ServiceProcess> _serviceProcesses = new();

    private INetwork? _network;
    private IContainer? _postgresContainer;
    private IContainer? _zookeeperContainer;
    private IContainer? _kafkaContainer;
    private IContainer? _eventStoreContainer;
    private IContainer? _mailpitContainer;

    private string? _postgresConnectionString;
    private string? _kafkaBootstrapServers;
    private string? _eventStoreHttpBaseUrl;
    private string? _eventStoreTcpConnectionString;
    private string? _mailpitBaseUrl;
    private int _mailpitSmtpPort;

    public AccountIntegrationEnvironment()
    {
        _repositoryRoot = FindRepositoryRoot();
        _artifactsRoot = Path.Combine(
            Path.GetTempPath(),
            "cleandddarchitecture-account-integration",
            Guid.NewGuid().ToString("N"));
        _startupLogPath = Path.Combine(_artifactsRoot, "startup.log");
    }

    public Uri AccountBaseAddress { get; private set; } = null!;

    public Uri GatewayBaseAddress { get; private set; } = null!;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_artifactsRoot);
        Log($"environment: initializing in '{_artifactsRoot}'");

        try
        {
            Log("environment: starting infrastructure");
            await StartInfrastructureAsync(cancellationToken).ConfigureAwait(false);
            Log("environment: building hosts");
            await BuildHostsAsync(cancellationToken).ConfigureAwait(false);
            Log("environment: starting hosts");
            await StartHostsAsync(cancellationToken).ConfigureAwait(false);
            Log("environment: ready");
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"Account integration environment failed to initialize. See startup log at '{_startupLogPath}'.",
                exception);
        }
    }

    public async Task<MailpitMessageDetail> WaitForMailAsync(
        string email,
        string subject,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await WaitForResultAsync(
                    async () =>
                    {
                        var response = await _mailpitClient.GetFromJsonAsync<MailpitMessagesResponse>(
                            $"{_mailpitBaseUrl}/api/v1/messages",
                            _jsonOptions,
                            cancellationToken).ConfigureAwait(false);

                        var message = response?.Messages?.FirstOrDefault(
                            candidate => string.Equals(candidate.Subject, subject, StringComparison.Ordinal)
                                && candidate.To.Any(recipient =>
                                    string.Equals(recipient.Address, email, StringComparison.OrdinalIgnoreCase)));

                        if (message is null)
                        {
                            return (false, (MailpitMessageDetail?)null);
                        }

                        var detail = await _mailpitClient.GetFromJsonAsync<MailpitMessageDetail>(
                            $"{_mailpitBaseUrl}/api/v1/message/{message.Id}",
                            _jsonOptions,
                            cancellationToken).ConfigureAwait(false);
                        return (detail is not null, detail);
                    },
                    timeout,
                    $"mail for '{email}'",
                    cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Mailpit did not return a message for {email}.");
        }
        catch (TimeoutException exception)
        {
            throw new TimeoutException(
                $"{exception.Message}{Environment.NewLine}{GetServiceLogs()}",
                exception);
        }
    }

    public static string ExtractConfirmationLink(MailpitMessageDetail message)
    {
        var match = ConfirmationUrlRegex().Match(message.Html ?? message.Text ?? string.Empty);
        if (!match.Success)
        {
            throw new InvalidOperationException(
                $"Could not extract a confirmation link from the email body.{Environment.NewLine}{message.Html ?? message.Text}");
        }

        return match.Groups["url"].Value;
    }

    public async Task<Guid> WaitForConfirmedUserAsync(
        string email,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_postgresConnectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        return await WaitForResultAsync(
                async () =>
                {
                    await using var command = connection.CreateCommand();
                    command.CommandText =
                        """
                        SELECT "Id"
                        FROM "AspNetUsers"
                        WHERE "Email" = @email AND "EmailConfirmed" = TRUE
                        """;
                    command.Parameters.AddWithValue("email", email);

                    var value = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    return value is Guid id ? (true, id) : (false, Guid.Empty);
                },
                timeout,
                $"confirmed user '{email}' in postgres",
                cancellationToken).ConfigureAwait(false)
            ;
    }

    public async Task<IReadOnlyCollection<string>> WaitForAccountEventsInEventStoreAsync(
        Guid userId,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        return await WaitForResultAsync(
                async () =>
                {
                    using var request = new HttpRequestMessage(
                        HttpMethod.Get,
                        $"{_eventStoreHttpBaseUrl}/streams/AccountAggregate_{userId}");
                    request.Headers.Accept.ParseAdd("application/vnd.eventstore.atom+json");
                    request.Headers.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("admin:changeit")));

                    using var response = await _eventStoreClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        return (false, Array.Empty<string>() as IReadOnlyCollection<string>);
                    }

                    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                    var payload = await JsonSerializer.DeserializeAsync<EventStoreStreamResponse>(
                        stream,
                        _jsonOptions,
                        cancellationToken).ConfigureAwait(false);

                    var summaries = payload?.Entries?
                        .Select(entry => entry.Summary)
                        .Where(summary => !string.IsNullOrWhiteSpace(summary))
                        .Cast<string>()
                        .ToArray();

                    return summaries is { Length: > 0 } summariesResult
                        && summariesResult.Contains("AccountCreatedDomainEvent", StringComparer.Ordinal)
                        && summariesResult.Contains("AccountEmailConfirmedDomainEvent", StringComparer.Ordinal)
                            ? (true, summariesResult as IReadOnlyCollection<string>)
                            : (false, Array.Empty<string>() as IReadOnlyCollection<string>);
                },
                timeout,
                $"event stream for account '{userId}'",
                cancellationToken).ConfigureAwait(false)
            ;
    }

    public async Task<IReadOnlyCollection<string>> WaitForKafkaAccountEventsAsync(
        string email,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        return await WaitForResultAsync(
                async () =>
                {
                    var messages = await ConsumeKafkaMessagesAsync(email, timeout, cancellationToken).ConfigureAwait(false);
                    return messages is not null
                        ? (true, messages)
                        : (false, Array.Empty<string>() as IReadOnlyCollection<string>);
                },
                timeout,
                $"kafka account events for '{email}'",
                cancellationToken).ConfigureAwait(false)
            ;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var process in _serviceProcesses.AsEnumerable().Reverse())
        {
            await process.DisposeAsync().ConfigureAwait(false);
        }

        if (_mailpitContainer is not null)
        {
            await _mailpitContainer.DisposeAsync().ConfigureAwait(false);
        }

        if (_eventStoreContainer is not null)
        {
            await _eventStoreContainer.DisposeAsync().ConfigureAwait(false);
        }

        if (_kafkaContainer is not null)
        {
            await _kafkaContainer.DisposeAsync().ConfigureAwait(false);
        }

        if (_zookeeperContainer is not null)
        {
            await _zookeeperContainer.DisposeAsync().ConfigureAwait(false);
        }

        if (_postgresContainer is not null)
        {
            await _postgresContainer.DisposeAsync().ConfigureAwait(false);
        }

        if (_network is not null)
        {
            await _network.DeleteAsync().ConfigureAwait(false);
        }

        try
        {
            if (Directory.Exists(_artifactsRoot))
            {
                Directory.Delete(_artifactsRoot, recursive: true);
            }
        }
        catch
        {
            // best effort cleanup
        }
    }

    private async Task StartInfrastructureAsync(CancellationToken cancellationToken)
    {
        var postgresPort = GetFreePort();
        var eventStoreHttpPort = GetFreePort();
        var eventStoreTcpPort = GetFreePort();
        var mailpitHttpPort = GetFreePort();
        var mailpitSmtpPort = GetFreePort();
        var kafkaExternalPort = GetFreePort();

        _network = new NetworkBuilder()
            .WithName($"cleandddarchitecture-integration-{Guid.NewGuid():N}")
            .Build();

        await _network.CreateAsync(cancellationToken).ConfigureAwait(false);

        _postgresContainer = new ContainerBuilder("postgres:16-alpine")
            .WithName($"account-it-postgres-{Guid.NewGuid():N}")
            .WithEnvironment("POSTGRES_DB", DatabaseName)
            .WithEnvironment("POSTGRES_USER", DatabaseUser)
            .WithEnvironment("POSTGRES_PASSWORD", DatabasePassword)
            .WithPortBinding(postgresPort, 5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(5432))
            .WithCleanUp(true)
            .Build();

        _zookeeperContainer = new ContainerBuilder("confluentinc/cp-zookeeper:7.6.1")
            .WithName($"account-it-zookeeper-{Guid.NewGuid():N}")
            .WithNetwork(_network)
            .WithNetworkAliases("zookeeper")
            .WithEnvironment("ZOOKEEPER_CLIENT_PORT", "2181")
            .WithEnvironment("ZOOKEEPER_TICK_TIME", "2000")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(2181))
            .WithCleanUp(true)
            .Build();

        _kafkaContainer = new ContainerBuilder("confluentinc/cp-kafka:7.6.1")
            .WithName($"account-it-kafka-{Guid.NewGuid():N}")
            .WithNetwork(_network)
            .WithNetworkAliases("kafka")
            .WithPortBinding(kafkaExternalPort, 19092)
            .WithEnvironment("KAFKA_BROKER_ID", "1")
            .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "zookeeper:2181")
            .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", $"PLAINTEXT://kafka:9092,PLAINTEXT_HOST://127.0.0.1:{kafkaExternalPort}")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "PLAINTEXT")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_LISTENERS", "PLAINTEXT://0.0.0.0:9092,PLAINTEXT_HOST://0.0.0.0:19092")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("cub kafka-ready -b localhost:9092 1 20"))
            .WithCleanUp(true)
            .Build();

        _eventStoreContainer = new ContainerBuilder("eventstore/eventstore:21.10.11-buster-slim")
            .WithName($"account-it-eventstore-{Guid.NewGuid():N}")
            .WithPortBinding(eventStoreHttpPort, 2113)
            .WithPortBinding(eventStoreTcpPort, 1113)
            .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
            .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
            .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
            .WithEnvironment("EVENTSTORE_INSECURE", "true")
            .WithEnvironment("EVENTSTORE_ENABLE_EXTERNAL_TCP", "true")
            .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
            .WithEnvironment("EVENTSTORE_EXT_TCP_PORT", "1113")
            .WithEnvironment("EVENTSTORE_HTTP_PORT", "2113")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(2113))
            .WithCleanUp(true)
            .Build();

        _mailpitContainer = new ContainerBuilder("axllent/mailpit:latest")
            .WithName($"account-it-mailpit-{Guid.NewGuid():N}")
            .WithPortBinding(mailpitHttpPort, 8025)
            .WithPortBinding(mailpitSmtpPort, 1025)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(8025))
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync(cancellationToken).ConfigureAwait(false);
        Log($"infrastructure: postgres container started on port {postgresPort}");
        await _zookeeperContainer.StartAsync(cancellationToken).ConfigureAwait(false);
        Log("infrastructure: zookeeper container started");
        await _kafkaContainer.StartAsync(cancellationToken).ConfigureAwait(false);
        Log($"infrastructure: kafka container started on port {kafkaExternalPort}");
        await _eventStoreContainer.StartAsync(cancellationToken).ConfigureAwait(false);
        Log($"infrastructure: eventstore container started on ports http={eventStoreHttpPort}, tcp={eventStoreTcpPort}");
        await _mailpitContainer.StartAsync(cancellationToken).ConfigureAwait(false);
        Log($"infrastructure: mailpit container started on ports http={mailpitHttpPort}, smtp={mailpitSmtpPort}");

        _postgresConnectionString =
            $"Host=127.0.0.1;Port={postgresPort};Database={DatabaseName};Username={DatabaseUser};Password={DatabasePassword}";
        _kafkaBootstrapServers = $"127.0.0.1:{kafkaExternalPort}";
        _eventStoreHttpBaseUrl = $"http://127.0.0.1:{eventStoreHttpPort}";
        _eventStoreTcpConnectionString = $"tcp://admin:changeit@127.0.0.1:{eventStoreTcpPort}";
        _mailpitBaseUrl = $"http://127.0.0.1:{mailpitHttpPort}";
        _mailpitSmtpPort = mailpitSmtpPort;

        await WaitForPostgresAsync(cancellationToken).ConfigureAwait(false);
        Log("infrastructure: postgres ready");
        await WaitForMailpitAsync(cancellationToken).ConfigureAwait(false);
        Log("infrastructure: mailpit ready");
        await WaitForEventStoreAsync(cancellationToken).ConfigureAwait(false);
        Log("infrastructure: eventstore ready");
        await WaitForKafkaAsync(cancellationToken).ConfigureAwait(false);
        Log("infrastructure: kafka ready");
    }

    private async Task BuildHostsAsync(CancellationToken cancellationToken)
    {
        await EnsureProjectBuiltAsync(
            "Hosts/Services/AccountService/Presentation/Presentation.csproj",
            "Hosts/Services/AccountService/Presentation",
            "CleanDDDArchitecture.Hosts.Services.AccountService.Presentation.dll",
            cancellationToken).ConfigureAwait(false);
        await EnsureProjectBuiltAsync(
            "Hosts/Services/TodoService/Presentation/Presentation.csproj",
            "Hosts/Services/TodoService/Presentation",
            "CleanDDDArchitecture.Hosts.Services.TodoService.Presentation.dll",
            cancellationToken).ConfigureAwait(false);
        await EnsureProjectBuiltAsync(
            "Hosts/Gateway/Presentation/Presentation.csproj",
            "Hosts/Gateway/Presentation",
            "CleanDDDArchitecture.Hosts.Gateway.Presentation.dll",
            cancellationToken).ConfigureAwait(false);
    }

    private async Task StartHostsAsync(CancellationToken cancellationToken)
    {
        var accountPort = GetFreePort();
        var todoPort = GetFreePort();
        var gatewayPort = GetFreePort();

        AccountBaseAddress = new Uri($"http://127.0.0.1:{accountPort}");
        GatewayBaseAddress = new Uri($"http://127.0.0.1:{gatewayPort}");

        var accountOutputPath = GetProjectOutputPath(
            "Hosts/Services/AccountService/Presentation",
            "CleanDDDArchitecture.Hosts.Services.AccountService.Presentation.dll");
        var todoOutputPath = GetProjectOutputPath(
            "Hosts/Services/TodoService/Presentation",
            "CleanDDDArchitecture.Hosts.Services.TodoService.Presentation.dll");
        var gatewayOutputPath = GetProjectOutputPath(
            "Hosts/Gateway/Presentation",
            "CleanDDDArchitecture.Hosts.Gateway.Presentation.dll");

        var accountService = new ServiceProcess(
            "account-service",
            accountOutputPath,
            Path.GetDirectoryName(accountOutputPath)!,
            new Uri(AccountBaseAddress, "health"),
            new Dictionary<string, string>
            {
                ["ASPNETCORE_URLS"] = AccountBaseAddress.ToString(),
                ["DOTNET_ENVIRONMENT"] = "Development",
                ["DataProtection__KeysPath"] = Path.Combine(_artifactsRoot, "account-dp"),
                ["AppSettings__BaseUrl"] = AccountBaseAddress.ToString().TrimEnd('/'),
                ["ConnectionStrings__PGSQLConnection"] = _postgresConnectionString!,
                ["ConnectionStrings__eventstore"] = _eventStoreTcpConnectionString!,
                ["ConnectionStrings__kafka"] = _kafkaBootstrapServers!,
                ["EmailSettings__SmtpHost"] = "127.0.0.1",
                ["EmailSettings:SmtpHost"] = "127.0.0.1",
                ["EmailSettings__SmtpPort"] = _mailpitSmtpPort.ToString(CultureInfo.InvariantCulture),
                ["EmailSettings:SmtpPort"] = _mailpitSmtpPort.ToString(CultureInfo.InvariantCulture),
                ["EmailSettings__EnableSsl"] = "false",
                ["EmailSettings:EnableSsl"] = "false",
                ["EmailSettings__SmtpUsername"] = string.Empty,
                ["EmailSettings:SmtpUsername"] = string.Empty,
                ["EmailSettings__SmtpPassword"] = string.Empty,
                ["EmailSettings:SmtpPassword"] = string.Empty,
                ["EventSourcing__EnableConsumer"] = "false",
                ["Jwt__Issuer"] = JwtIssuer,
                ["Jwt__Audience"] = JwtAudience,
                ["Jwt__Access__Key256Bit"] = JwtAccessKey256,
                ["Jwt__Access__Key512Bit"] = JwtAccessKey512,
                ["Jwt__Access__ExpirationDurationInMinutes"] = "15",
                ["Jwt__Refresh__Key256Bit"] = JwtRefreshKey256,
                ["Jwt__Refresh__Key512Bit"] = JwtRefreshKey512,
                ["Jwt__Refresh__ExpirationDurationInMinutes"] = "10080",
                ["Jwt__ClockSkewInMinutes"] = "1"
            },
            Log);

        var todoService = new ServiceProcess(
            "todo-service",
            todoOutputPath,
            Path.GetDirectoryName(todoOutputPath)!,
            new Uri($"http://127.0.0.1:{todoPort}/health"),
            new Dictionary<string, string>
            {
                ["ASPNETCORE_URLS"] = $"http://127.0.0.1:{todoPort}",
                ["DOTNET_ENVIRONMENT"] = "Development",
                ["DataProtection__KeysPath"] = Path.Combine(_artifactsRoot, "todo-dp"),
                ["ConnectionStrings__PGSQLConnection"] = _postgresConnectionString!,
                ["Jwt__Issuer"] = JwtIssuer,
                ["Jwt__Audience"] = JwtAudience,
                ["Jwt__Access__Key256Bit"] = JwtAccessKey256,
                ["Jwt__Access__Key512Bit"] = JwtAccessKey512,
                ["Jwt__ClockSkewInMinutes"] = "1"
            },
            Log);

        var gatewayService = new ServiceProcess(
            "gateway",
            gatewayOutputPath,
            Path.GetDirectoryName(gatewayOutputPath)!,
            new Uri(GatewayBaseAddress, "health"),
            new Dictionary<string, string>
            {
                ["ASPNETCORE_URLS"] = GatewayBaseAddress.ToString(),
                ["DOTNET_ENVIRONMENT"] = "Development",
                ["ReverseProxy__Clusters__account__Destinations__primary__Address"] = AccountBaseAddress.ToString(),
                ["ReverseProxy__Clusters__todo__Destinations__primary__Address"] = $"http://127.0.0.1:{todoPort}/",
                ["ReverseProxy__Clusters__weather__Destinations__primary__Address"] = $"http://127.0.0.1:{todoPort}/"
            },
            Log);

        _serviceProcesses.Add(accountService);
        _serviceProcesses.Add(todoService);
        _serviceProcesses.Add(gatewayService);

        Log("hosts: starting account-service");
        await accountService.StartAsync(TimeSpan.FromMinutes(2), cancellationToken).ConfigureAwait(false);
        Log("hosts: account-service healthy");
        Log("hosts: starting todo-service");
        await todoService.StartAsync(TimeSpan.FromMinutes(2), cancellationToken).ConfigureAwait(false);
        Log("hosts: todo-service healthy");
        Log("hosts: starting gateway");
        await gatewayService.StartAsync(TimeSpan.FromMinutes(1), cancellationToken).ConfigureAwait(false);
        Log("hosts: gateway healthy");
    }

    private async Task WaitForPostgresAsync(CancellationToken cancellationToken)
    {
        await WaitForConditionAsync(
                async () =>
                {
                    try
                    {
                        await using var connection = new NpgsqlConnection(_postgresConnectionString);
                        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                TimeSpan.FromMinutes(1),
                "postgres readiness",
                cancellationToken).ConfigureAwait(false);
    }

    private async Task WaitForMailpitAsync(CancellationToken cancellationToken)
    {
        await WaitForConditionAsync(
                async () =>
                {
                    try
                    {
                        using var response = await _mailpitClient.GetAsync(
                            $"{_mailpitBaseUrl}/api/v1/messages",
                            cancellationToken).ConfigureAwait(false);
                        return response.IsSuccessStatusCode;
                    }
                    catch
                    {
                        return false;
                    }
                },
                TimeSpan.FromMinutes(1),
                "mailpit readiness",
                cancellationToken).ConfigureAwait(false);
    }

    private async Task WaitForEventStoreAsync(CancellationToken cancellationToken)
    {
        await WaitForConditionAsync(
                async () =>
                {
                    try
                    {
                        using var response = await _eventStoreClient.GetAsync(
                            $"{_eventStoreHttpBaseUrl}/health/live",
                            cancellationToken).ConfigureAwait(false);
                        return response.IsSuccessStatusCode;
                    }
                    catch
                    {
                        return false;
                    }
                },
                TimeSpan.FromMinutes(1),
                "eventstore readiness",
                cancellationToken).ConfigureAwait(false);
    }

    private static async Task WaitForKafkaAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyCollection<string>?> ConsumeKafkaMessagesAsync(
        string email,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(
                new ConsumerConfig
                {
                    BootstrapServers = _kafkaBootstrapServers,
                    GroupId = $"account-it-{Guid.NewGuid():N}",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                })
            .Build();

        consumer.Subscribe(KafkaTopic);

        List<string> messages = [];
        var deadline = DateTimeOffset.UtcNow + timeout;

        while (DateTimeOffset.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
            if (consumeResult is null)
            {
                continue;
            }

            messages.Add(consumeResult.Message.Value);

            var hasCreated = messages.Any(message =>
                message.Contains("\"Email\":\"" + email + "\"", StringComparison.Ordinal)
                && message.Contains("\"EmailConfirmed\":false", StringComparison.Ordinal));
            var hasConfirmed = messages.Any(message =>
                message.Contains("\"Email\":\"" + email + "\"", StringComparison.Ordinal)
                && message.Contains("\"EmailConfirmed\":true", StringComparison.Ordinal));

            if (hasCreated && hasConfirmed)
            {
                return messages;
            }
        }

        return null;
    }

    private async Task BuildProjectAsync(string relativeProjectPath, CancellationToken cancellationToken)
    {
        Log($"build: starting {relativeProjectPath}");
        var projectPath = Path.Combine(_repositoryRoot, relativeProjectPath);
        using Process process = new();
        process.StartInfo = new ProcessStartInfo(
            "dotnet",
            $"build \"{projectPath}\" --configuration Release --nologo --no-restore")
        {
            WorkingDirectory = _repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        process.Start();
        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        var waitForExitTask = process.WaitForExitAsync(cancellationToken);

        await Task.WhenAll(outputTask, errorTask, waitForExitTask).ConfigureAwait(false);

        var output = await outputTask.ConfigureAwait(false);
        var error = await errorTask.ConfigureAwait(false);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Failed to build '{relativeProjectPath}'.{Environment.NewLine}{output}{Environment.NewLine}{error}");
        }
        Log($"build: completed {relativeProjectPath}");
    }

    private async Task EnsureProjectBuiltAsync(
        string relativeProjectPath,
        string relativeProjectDirectory,
        string assemblyName,
        CancellationToken cancellationToken)
    {
        var outputPath = GetProjectOutputPath(relativeProjectDirectory, assemblyName);
        if (File.Exists(outputPath))
        {
            Log($"build: reusing existing output for {relativeProjectPath} at '{outputPath}'");
            return;
        }

        await BuildProjectAsync(relativeProjectPath, cancellationToken).ConfigureAwait(false);
    }

    private string GetProjectOutputPath(string relativeProjectDirectory, string assemblyName)
    {
        var outputDirectory = Path.Combine(
            _repositoryRoot,
            relativeProjectDirectory,
            "bin",
            "Release",
            "net10.0");

        return Path.Combine(outputDirectory, assemblyName);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CleanDDDArchitecture.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the CleanDDDArchitecture repository root.");
    }

    private static int GetFreePort()
    {
        using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        return ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static async Task WaitForConditionAsync(
        Func<Task<bool>> predicate,
        TimeSpan timeout,
        string description,
        CancellationToken cancellationToken)
    {
        var deadline = DateTimeOffset.UtcNow + timeout;

        while (DateTimeOffset.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if (await predicate().ConfigureAwait(false))
                {
                    return;
                }
            }
            catch
            {
                // retry until timeout
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
        }

        throw new TimeoutException($"Timed out waiting for {description}.");
    }

    private static async Task<T> WaitForResultAsync<T>(
        Func<Task<(bool Success, T Result)>> action,
        TimeSpan timeout,
        string description,
        CancellationToken cancellationToken)
    {
        var deadline = DateTimeOffset.UtcNow + timeout;

        while (DateTimeOffset.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var (success, result) = await action().ConfigureAwait(false);
                if (success)
                {
                    return result;
                }
            }
            catch
            {
                // retry until timeout
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
        }

        throw new TimeoutException($"Timed out waiting for {description}.");
    }

    private void Log(string message)
    {
        var line = $"{DateTimeOffset.UtcNow:O} {message}";
        Console.WriteLine(line);
        File.AppendAllLines(_startupLogPath, [line]);
    }

    private string GetServiceLogs()
    {
        if (_serviceProcesses.Count == 0)
        {
            return "No service processes were started.";
        }

        return string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            _serviceProcesses.Select(
                process => process.GetLogs()));
    }

    [GeneratedRegex("href='(?<url>[^']+)'", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ConfirmationUrlRegex();

    private sealed class MailpitMessagesResponse
    {
        public List<MailpitMessageSummary> Messages { get; set; } = [];
    }

    private sealed class MailpitMessageSummary
    {
        public required string Id { get; set; }

        public required string Subject { get; set; }

        public List<MailpitAddress> To { get; set; } = [];
    }

    private sealed class MailpitAddress
    {
        public required string Address { get; set; }
    }

    internal sealed class MailpitMessageDetail
    {
        public string? Html { get; set; }

        public string? Text { get; set; }
    }

    private sealed class EventStoreStreamResponse
    {
        public List<EventStoreStreamEntry> Entries { get; set; } = [];
    }

    private sealed class EventStoreStreamEntry
    {
        public string? Summary { get; set; }
    }
}
