using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CleanDDDArchitecture.Domains.Account.Tests.Integration.Infrastructure;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Integration;

public sealed class AccountGatewayTodoFlowTests : IAsyncLifetime
{
    private const string TodoListTitle = "Grocery";
    private const string TodoItemTitle = "Apples";

    private readonly AccountIntegrationEnvironment _environment = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async ValueTask InitializeAsync()
    {
        using var cancellationTokenSource = new CancellationTokenSource(GetInitializationTimeout());
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

        var authPayload = await CreateConfirmedUserAndAuthenticateAsync(
            accountClient,
            gatewayClient,
            email,
            password,
            cancellationToken);
        var userId = await _environment.WaitForConfirmedUserAsync(
            email,
            TimeSpan.FromSeconds(30),
            cancellationToken);

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
                title = TodoListTitle
            },
            cancellationToken);

        var createTodoListContent = await createTodoListResponse.Content.ReadAsStringAsync(cancellationToken);
        createTodoListResponse.StatusCode.Should()
            .Be(
                HttpStatusCode.OK,
                because: $"gateway todo-list create returned: {createTodoListContent}");
        var createdTodoList = await createTodoListResponse.Content.ReadFromJsonAsync<CreatedTodoListResponse>(_jsonOptions, cancellationToken);
        createdTodoList.Should().NotBeNull();
        createdTodoList!.Id.Should().BeGreaterThan(0);

        var createTodoItemResponse = await gatewayClient.PostAsJsonAsync(
            "/api/todo/item",
            new
            {
                listId = createdTodoList.Id,
                title = TodoItemTitle
            },
            cancellationToken);

        createTodoItemResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var todoListsResponse = await gatewayClient.GetAsync("/api/todo/list", cancellationToken);
        var todoListsContent = await todoListsResponse.Content.ReadAsStringAsync(cancellationToken);
        todoListsResponse.StatusCode.Should()
            .Be(
                HttpStatusCode.OK,
                because: $"gateway todo-list get returned: {todoListsContent}");
        var todoListsPayload = await todoListsResponse.Content.ReadFromJsonAsync<TodoListsPayload>(_jsonOptions, cancellationToken);

        todoListsPayload.Should().NotBeNull();
        todoListsPayload!.Lists
            .Should()
            .Contain(list => list.Id == createdTodoList.Id && list.Title == TodoListTitle);
        todoListsPayload.Lists
            .Single(list => list.Id == createdTodoList.Id)
            .Items.Should()
            .Contain(item => item.Title == TodoItemTitle);
    }

    [Fact]
    public async Task RefreshLogoutAndPasswordLifecycleWorkThroughGateway()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var email = $"security.{Guid.NewGuid():N}@example.local";
        const string password = "Passw0rd!";
        const string resetPassword = "Reset1!";
        const string changedPassword = "Change1!";

        using HttpClient accountClient = new() { BaseAddress = _environment.AccountBaseAddress };
        using HttpClient gatewayClient = new() { BaseAddress = _environment.GatewayBaseAddress };

        var authPayload = await CreateConfirmedUserAndAuthenticateAsync(
            accountClient,
            gatewayClient,
            email,
            password,
            cancellationToken);

        authPayload.RefreshToken.Should().NotBeNullOrWhiteSpace();
        authPayload.AccessToken.Should().NotBeNullOrWhiteSpace();

        var refreshResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/refresh",
            new { refreshToken = authPayload.RefreshToken },
            cancellationToken);

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshedPayload = await refreshResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        refreshedPayload.Should().NotBeNull();
        refreshedPayload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        refreshedPayload.RefreshToken.Should().NotBeNullOrWhiteSpace();
        refreshedPayload.RefreshToken.Should().NotBe(authPayload.RefreshToken);

        var replayRefreshResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/refresh",
            new { refreshToken = authPayload.RefreshToken },
            cancellationToken);
        replayRefreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        gatewayClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", refreshedPayload.AccessToken);

        var sessionsResponse = await gatewayClient.GetAsync("/api/identity/sessions", cancellationToken);
        sessionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var sessions = await sessionsResponse.Content.ReadFromJsonAsync<List<AccountSessionResponse>>(_jsonOptions, cancellationToken);
        sessions.Should().NotBeNull();
        sessions.Should().OnlyContain(session => session.Id != Guid.Empty);

        var forgotPasswordResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/forgot-password",
            new { email },
            cancellationToken);
        forgotPasswordResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var resetMail = await _environment.WaitForMailAsync(
            email,
            "Reset your password",
            TimeSpan.FromSeconds(60),
            cancellationToken);
        var resetToken = AccountIntegrationEnvironment.ExtractPasswordResetToken(resetMail);

        var resetPasswordResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/reset-password",
            new
            {
                email,
                token = resetToken,
                password = resetPassword
            },
            cancellationToken);
        resetPasswordResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var oldPasswordAuthenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password
            },
            cancellationToken);
        oldPasswordAuthenticationResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var resetAuthenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password = resetPassword
            },
            cancellationToken);
        resetAuthenticationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var resetAuthPayload = await resetAuthenticationResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        resetAuthPayload.Should().NotBeNull();
        resetAuthPayload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        resetAuthPayload.RefreshToken.Should().NotBeNullOrWhiteSpace();

        gatewayClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", resetAuthPayload.AccessToken);

        var changePasswordResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/change-password",
            new
            {
                currentPassword = resetPassword,
                newPassword = changedPassword
            },
            cancellationToken);
        changePasswordResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var staleRefreshAfterPasswordChangeResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/refresh",
            new { refreshToken = resetAuthPayload.RefreshToken },
            cancellationToken);
        staleRefreshAfterPasswordChangeResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var resetPasswordAuthenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password = resetPassword
            },
            cancellationToken);
        resetPasswordAuthenticationResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var changedPasswordAuthenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password = changedPassword
            },
            cancellationToken);
        changedPasswordAuthenticationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var changedPasswordPayload = await changedPasswordAuthenticationResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        changedPasswordPayload.Should().NotBeNull();
        changedPasswordPayload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        changedPasswordPayload.RefreshToken.Should().NotBeNullOrWhiteSpace();

        var logoutResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/logout",
            new { refreshToken = changedPasswordPayload.RefreshToken },
            cancellationToken);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var refreshAfterLogoutResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/refresh",
            new { refreshToken = changedPasswordPayload.RefreshToken },
            cancellationToken);
        refreshAfterLogoutResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<AuthenticateResponse> CreateConfirmedUserAndAuthenticateAsync(
        HttpClient accountClient,
        HttpClient gatewayClient,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
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
            TimeSpan.FromSeconds(60),
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

        await _environment.WaitForConfirmedUserAsync(
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
        return authPayload;
    }

    private sealed class AuthenticateResponse
    {
        public string? Error { get; set; }

        [JsonPropertyName("confirm_token")]
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

    private sealed class TodoListsPayload
    {
        public List<TodoListResponse> Lists { get; set; } = [];
    }

    private sealed class AccountSessionResponse
    {
        public Guid Id { get; set; }
    }

    private static TimeSpan GetInitializationTimeout()
    {
        const int defaultTimeoutSeconds = 300;
        var value = Environment.GetEnvironmentVariable("ACCOUNT_INTEGRATION_INIT_TIMEOUT_SECONDS");

        return int.TryParse(value, out var seconds) && seconds > 0
            ? TimeSpan.FromSeconds(seconds)
            : TimeSpan.FromSeconds(defaultTimeoutSeconds);
    }
}
