using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
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

    [Fact]
    public async Task MfaPhoneEmailChangeAndDeletionLifecycleWorkThroughGateway()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var email = $"lifecycle.{Guid.NewGuid():N}@example.local";
        var newEmail = $"updated.{Guid.NewGuid():N}@example.local";
        const string password = "Passw0rd!";
        const string phoneNumber = "+306900000001";

        using HttpClient accountClient = new() { BaseAddress = _environment.AccountBaseAddress };
        using HttpClient gatewayClient = new() { BaseAddress = _environment.GatewayBaseAddress };

        var authPayload = await CreateConfirmedUserAndAuthenticateAsync(
            accountClient,
            gatewayClient,
            email,
            password,
            cancellationToken);

        gatewayClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authPayload.AccessToken);

        var mfaSetupResponse = await gatewayClient.PostAsync("/api/identity/mfa/totp/setup", null, cancellationToken);
        mfaSetupResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var mfaSetup = await mfaSetupResponse.Content.ReadFromJsonAsync<MfaSetupResponse>(_jsonOptions, cancellationToken);
        mfaSetup.Should().NotBeNull();
        mfaSetup!.SharedKey.Should().NotBeNullOrWhiteSpace();
        mfaSetup.AuthenticatorUri.Should().NotBeNullOrWhiteSpace();

        var totpCode = GenerateTotpCode(mfaSetup.SharedKey);
        var mfaVerifyResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/mfa/totp/verify",
            new { code = totpCode },
            cancellationToken);
        mfaVerifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var recoveryCodes = await mfaVerifyResponse.Content.ReadFromJsonAsync<MfaRecoveryCodesResponse>(_jsonOptions, cancellationToken);
        recoveryCodes.Should().NotBeNull();
        recoveryCodes!.RecoveryCodes.Should().NotBeEmpty();

        gatewayClient.DefaultRequestHeaders.Authorization = null;

        var mfaChallengeResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password
            },
            cancellationToken);
        mfaChallengeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var challengePayload = await mfaChallengeResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        challengePayload.Should().NotBeNull();
        challengePayload!.RequiresTwoFactor.Should().BeTrue();
        challengePayload.ChallengeType.Should().Be("totp");

        var trustedDeviceAuthenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password,
                recoveryCode = recoveryCodes.RecoveryCodes.First(),
                rememberDevice = true,
                deviceName = "Integration Device"
            },
            cancellationToken);
        trustedDeviceAuthenticationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var trustedDevicePayload = await trustedDeviceAuthenticationResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        trustedDevicePayload.Should().NotBeNull();
        trustedDevicePayload!.TrustedDeviceToken.Should().NotBeNullOrWhiteSpace();

        var trustedDeviceLoginResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password,
                trustedDeviceToken = trustedDevicePayload.TrustedDeviceToken
            },
            cancellationToken);
        trustedDeviceLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var trustedDeviceLoginPayload = await trustedDeviceLoginResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        trustedDeviceLoginPayload.Should().NotBeNull();
        trustedDeviceLoginPayload!.AccessToken.Should().NotBeNullOrWhiteSpace();

        gatewayClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", trustedDeviceLoginPayload.AccessToken);

        var trustedDevicesResponse = await gatewayClient.GetAsync("/api/identity/trusted-devices", cancellationToken);
        var trustedDevicesContent = await trustedDevicesResponse.Content.ReadAsStringAsync(cancellationToken);
        trustedDevicesResponse.StatusCode.Should().Be(
            HttpStatusCode.OK,
            because: $"trusted-devices returned: {trustedDevicesContent}");
        var trustedDevices = await trustedDevicesResponse.Content
            .ReadFromJsonAsync<TrustedDevicesResponse>(_jsonOptions, cancellationToken);
        trustedDevices.Should().NotBeNull();
        trustedDevices!.TrustedDevices.Should().ContainSingle();

        var revokeTrustedDeviceResponse = await gatewayClient.DeleteAsync(
            $"/api/identity/trusted-devices/{trustedDevices.TrustedDevices.Single().Id}",
            cancellationToken);
        revokeTrustedDeviceResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        gatewayClient.DefaultRequestHeaders.Authorization = null;

        var revokedTrustedDeviceLoginResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password,
                trustedDeviceToken = trustedDevicePayload.TrustedDeviceToken
            },
            cancellationToken);
        revokedTrustedDeviceLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var revokedTrustedDevicePayload = await revokedTrustedDeviceLoginResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        revokedTrustedDevicePayload.Should().NotBeNull();
        revokedTrustedDevicePayload!.RequiresTwoFactor.Should().BeTrue();

        var currentTotpCode = GenerateTotpCode(mfaSetup.SharedKey);
        var phoneAuthenticationResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password,
                twoFactorCode = currentTotpCode
            },
            cancellationToken);
        phoneAuthenticationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var phoneAuthPayload = await phoneAuthenticationResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        phoneAuthPayload.Should().NotBeNull();

        gatewayClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", phoneAuthPayload!.AccessToken);

        var phoneRequestResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/phone/request",
            new { phoneNumber },
            cancellationToken);
        phoneRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var phoneVerifyResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/phone/verify",
            new
            {
                phoneNumber,
                code = "123456"
            },
            cancellationToken);
        phoneVerifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var changeEmailRequestResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/change-email/request",
            new { newEmail },
            cancellationToken);
        changeEmailRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var changeEmailMail = await _environment.WaitForMailAsync(
            newEmail,
            "Confirm your new email",
            TimeSpan.FromSeconds(60),
            cancellationToken);
        var changeEmailLink = AccountIntegrationEnvironment.ExtractConfirmationLink(changeEmailMail);

        var changeEmailConfirmResponse = await accountClient.GetAsync(changeEmailLink, cancellationToken);
        changeEmailConfirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var oldEmailLoginResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = email,
                password,
                twoFactorCode = GenerateTotpCode(mfaSetup.SharedKey)
            },
            cancellationToken);
        oldEmailLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var newEmailLoginResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = newEmail,
                password,
                twoFactorCode = GenerateTotpCode(mfaSetup.SharedKey)
            },
            cancellationToken);
        newEmailLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var newEmailAuthPayload = await newEmailLoginResponse.Content
            .ReadFromJsonAsync<AuthenticateResponse>(_jsonOptions, cancellationToken);
        newEmailAuthPayload.Should().NotBeNull();

        gatewayClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", newEmailAuthPayload!.AccessToken);

        var securityEventsResponse = await gatewayClient.GetAsync("/api/identity/security-events", cancellationToken);
        securityEventsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var securityEvents = await securityEventsResponse.Content
            .ReadFromJsonAsync<AccountSecurityEventsResponse>(_jsonOptions, cancellationToken);
        securityEvents.Should().NotBeNull();
        securityEvents!.Events.Should().Contain(evt => evt.Type.Contains("Phone", StringComparison.OrdinalIgnoreCase));
        securityEvents.Events.Should().Contain(evt => evt.Type.Contains("Email", StringComparison.OrdinalIgnoreCase));

        var deleteRequestResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/delete/request",
            new { currentPassword = password },
            cancellationToken);
        deleteRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var deleteMail = await _environment.WaitForMailAsync(
            newEmail,
            "Confirm account deletion",
            TimeSpan.FromSeconds(60),
            cancellationToken);
        var deleteConfirmationLink = AccountIntegrationEnvironment.ExtractConfirmationLink(deleteMail);

        var deleteConfirmResponse = await accountClient.GetAsync(deleteConfirmationLink, cancellationToken);
        deleteConfirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedLoginResponse = await gatewayClient.PostAsJsonAsync(
            "/api/identity/authenticate",
            new
            {
                username = newEmail,
                password,
                twoFactorCode = GenerateTotpCode(mfaSetup.SharedKey)
            },
            cancellationToken);
        deletedLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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

        public bool RequiresTwoFactor { get; set; }

        public string? ChallengeType { get; set; }

        public string? TrustedDeviceToken { get; set; }
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

    private sealed class MfaSetupResponse
    {
        public string SharedKey { get; set; } = string.Empty;

        public string AuthenticatorUri { get; set; } = string.Empty;
    }

    private sealed class MfaRecoveryCodesResponse
    {
        public List<string> RecoveryCodes { get; set; } = [];
    }

    private sealed class TrustedDevicesResponse
    {
        public List<TrustedDeviceResponse> TrustedDevices { get; set; } = [];
    }

    private sealed class TrustedDeviceResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class AccountSecurityEventsResponse
    {
        public List<AccountSecurityEventResponse> Events { get; set; } = [];
    }

    private sealed class AccountSecurityEventResponse
    {
        public string Type { get; set; } = string.Empty;
    }

    private static TimeSpan GetInitializationTimeout()
    {
        const int defaultTimeoutSeconds = 300;
        var value = Environment.GetEnvironmentVariable("ACCOUNT_INTEGRATION_INIT_TIMEOUT_SECONDS");

        return int.TryParse(value, out var seconds) && seconds > 0
            ? TimeSpan.FromSeconds(seconds)
            : TimeSpan.FromSeconds(defaultTimeoutSeconds);
    }

    private static string GenerateTotpCode(string sharedKey)
    {
        var normalizedKey = sharedKey.Replace(" ", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
        var key = DecodeBase32(normalizedKey);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        Span<byte> counter = stackalloc byte[8];

        for (var index = 7; index >= 0; index--)
        {
            counter[index] = (byte)(timestamp & 0xff);
            timestamp >>= 8;
        }

#pragma warning disable CA5350 // TOTP interoperability requires HMAC-SHA1.
        using var hmac = new HMACSHA1(key);
#pragma warning restore CA5350
        var hash = hmac.ComputeHash(counter.ToArray());
        var offset = hash[^1] & 0x0f;
        var binaryCode = ((hash[offset] & 0x7f) << 24)
                       | (hash[offset + 1] << 16)
                       | (hash[offset + 2] << 8)
                       | hash[offset + 3];

        return (binaryCode % 1_000_000).ToString("D6", CultureInfo.InvariantCulture);
    }

    private static byte[] DecodeBase32(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var cleaned = input.TrimEnd('=');
        var output = new List<byte>(cleaned.Length * 5 / 8);
        var bitBuffer = 0;
        var bitsInBuffer = 0;

        foreach (var character in cleaned)
        {
            var value = alphabet.IndexOf(character, StringComparison.Ordinal);
            if (value < 0)
            {
                throw new FormatException($"Invalid base32 character '{character}'.");
            }

            bitBuffer = (bitBuffer << 5) | value;
            bitsInBuffer += 5;

            if (bitsInBuffer >= 8)
            {
                bitsInBuffer -= 8;
                output.Add((byte)((bitBuffer >> bitsInBuffer) & 0xff));
            }
        }

        return [.. output];
    }
}
