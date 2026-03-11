using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CleanDDDArchitecture.Domains.Account.Tests.Integration.Infrastructure;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Integration;

public sealed class AccountGatewayTodoFlowTests : IAsyncLifetime
{
    private readonly AccountIntegrationEnvironment _environment = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async ValueTask InitializeAsync()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        await _environment.InitializeAsync(cancellationTokenSource.Token);
    }

    public ValueTask DisposeAsync() => _environment.DisposeAsync();

    [Fact]
    public async Task SignupConfirmLoginAndManageTodoThroughGateway()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var email = $"integration.{Guid.NewGuid():N}@example.local";
        const string password = "Passw0rd!";

        using HttpClient accountClient = new() { BaseAddress = _environment.AccountBaseAddress };
        using HttpClient gatewayClient = new() { BaseAddress = _environment.GatewayBaseAddress };

        var createResponse = await accountClient.PostAsJsonAsync(
            "/api/identity",
            new
            {
                password,
                firstName = "Integration",
                lastName = "Tester",
                email
            },
            cancellationToken);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var confirmationMail = await _environment.WaitForMailAsync(
            email,
            "Confirm your email",
            TimeSpan.FromSeconds(30),
            cancellationToken);
        var confirmationLink = AccountIntegrationEnvironment.ExtractConfirmationLink(confirmationMail);

        var preConfirmAuthenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password
            },
            cancellationToken);

        preConfirmAuthenticationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var preConfirmPayload = await preConfirmAuthenticationResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        preConfirmPayload.Should().NotBeNull();
        preConfirmPayload!.Error.Should().Be("Confirm your email first");
        preConfirmPayload.ConfirmToken.Should().NotBeNullOrWhiteSpace();

        var confirmResponse = await accountClient.GetAsync(confirmationLink, cancellationToken);
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var userId = await _environment.WaitForConfirmedUserAsync(
            email,
            TimeSpan.FromSeconds(30),
            cancellationToken);

        var authenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password
            },
            cancellationToken);

        authenticationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var authPayload = await authenticationResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);

        authPayload.Should().NotBeNull();
        authPayload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        authPayload.RefreshToken.Should().NotBeNullOrWhiteSpace();

        gatewayClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authPayload.AccessToken);

        var profileResponse = await gatewayClient.GetAsync("/api/identity/profile", cancellationToken);
        profileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = await profileResponse.Content.ReadFromJsonAsync<AccountProfileResponse>(_jsonOptions, cancellationToken);
        profile.Should().NotBeNull();
        profile!.Email.Should().Be(email);

        var eventStoreEntries = await _environment.WaitForAccountEventsInEventStoreAsync(
            userId,
            TimeSpan.FromSeconds(30),
            cancellationToken);
        eventStoreEntries.Should().Contain("AccountCreatedDomainEvent");
        eventStoreEntries.Should().Contain("AccountEmailConfirmedDomainEvent");

        var kafkaMessages = await _environment.WaitForKafkaAccountEventsAsync(
            email,
            TimeSpan.FromSeconds(30),
            cancellationToken);
        kafkaMessages.Should().Contain(message => message.Contains("\"EmailConfirmed\":false", StringComparison.Ordinal));
        kafkaMessages.Should().Contain(message => message.Contains("\"EmailConfirmed\":true", StringComparison.Ordinal));

        var createTodoListResponse = await gatewayClient.PostAsJsonAsync(
            "/api/todo/list",
            new
            {
                title = "Integration list"
            },
            cancellationToken);

        createTodoListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdTodoList = await createTodoListResponse.Content.ReadFromJsonAsync<CreatedTodoListResponse>(_jsonOptions, cancellationToken);
        createdTodoList.Should().NotBeNull();
        createdTodoList!.Id.Should().BeGreaterThan(0);

        var createTodoItemResponse = await gatewayClient.PostAsJsonAsync(
            "/api/todo/item",
            new
            {
                listId = createdTodoList.Id,
                title = "Integration item"
            },
            cancellationToken);

        createTodoItemResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var todoListsResponse = await gatewayClient.GetAsync("/api/todo/list", cancellationToken);
        todoListsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoLists = await todoListsResponse.Content.ReadFromJsonAsync<List<TodoListResponse>>(_jsonOptions, cancellationToken);

        todoLists.Should().NotBeNull();
        todoLists!
            .Should()
            .Contain(list => list.Id == createdTodoList.Id && list.Title == "Integration list");
        todoLists
            .Single(list => list.Id == createdTodoList.Id)
            .Items.Should()
            .Contain(item => item.Title == "Integration item");
    }

    private sealed class AuthenticateResponse
    {
        public string? Error { get; set; }

        public string? ConfirmToken { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }
    }

    private sealed class AccountProfileResponse
    {
        public string? Email { get; set; }
    }

    private sealed class CreatedTodoListResponse
    {
        public int Id { get; set; }

        public string? Title { get; set; }
    }

    private sealed class TodoListResponse
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public List<TodoItemResponse> Items { get; set; } = [];
    }

    private sealed class TodoItemResponse
    {
        public string? Title { get; set; }
    }
}
