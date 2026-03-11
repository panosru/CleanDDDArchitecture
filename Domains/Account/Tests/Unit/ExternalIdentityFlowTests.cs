using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalBegin;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalComplete;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalLogins;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalProviders;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalUnlink;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class ExternalIdentityFlowTests
{
    [Fact]
    public async Task ExternalProvidersQueryShouldReturnConfiguredProviders()
    {
        var service = new CapturingAuthenticationService();
        var handler = new ExternalProvidersQuery.ExternalProvidersQueryHandler(service);

        var result = await handler.Handle(new ExternalProvidersQuery(), CancellationToken.None);

        result.Should().ContainSingle();
        result.Single().Provider.Should().Be("google");
    }

    [Fact]
    public async Task ExternalBeginCommandShouldPassCurrentUserWhenAvailable()
    {
        var service = new CapturingAuthenticationService();
        var currentUserId = Guid.Parse("ef1b7db6-3c75-4f6f-9a5e-2b2c76aa6f44");
        var handler = new ExternalBeginCommand.ExternalBeginCommandHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var response = await handler.Handle(
            new ExternalBeginCommand("google", "https://app.example.com/callback"),
            CancellationToken.None);

        response.Should().NotBeNull();
        service.BeginProvider.Should().Be("google");
        service.BeginRedirectUri.Should().Be("https://app.example.com/callback");
        service.BeginUserId.Should().Be(currentUserId);
    }

    [Fact]
    public async Task ExternalCompleteCommandShouldReturnTokensAndCurrentUser()
    {
        var service = new CapturingAuthenticationService();
        var currentUserId = Guid.Parse("59f19b47-ec17-4b27-94ad-5904f10962f9");
        var handler = new ExternalCompleteCommand.ExternalCompleteCommandHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var response = await handler.Handle(
            new ExternalCompleteCommand(
                "google",
                "auth-code",
                "state-value",
                "https://app.example.com/callback"),
            CancellationToken.None);

        response.Should().NotBeNull();
        response!.Tokens.Should().NotBeNull();
        service.CompleteProvider.Should().Be("google");
        service.CompleteCode.Should().Be("auth-code");
        service.CompleteState.Should().Be("state-value");
        service.CompleteRedirectUri.Should().Be("https://app.example.com/callback");
        service.CompleteUserId.Should().Be(currentUserId);
    }

    [Fact]
    public async Task ExternalLoginsQueryShouldUseCurrentUser()
    {
        var service = new CapturingAuthenticationService();
        var currentUserId = Guid.Parse("761f93b4-cd9f-471f-81f0-61fdf8999c34");
        var handler = new ExternalLoginsQuery.ExternalLoginsQueryHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new ExternalLoginsQuery(), CancellationToken.None);

        result.Should().ContainSingle();
        service.LoginsUserId.Should().Be(currentUserId);
    }

    [Fact]
    public async Task ExternalUnlinkCommandShouldUseCurrentUser()
    {
        var service = new CapturingAuthenticationService();
        var currentUserId = Guid.Parse("6f23b35f-faf3-4379-9586-38e9857ba650");
        var handler = new ExternalUnlinkCommand.ExternalUnlinkCommandHandler(
            service,
            new StubCurrentUserService(currentUserId));

        var result = await handler.Handle(new ExternalUnlinkCommand("google"), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        service.UnlinkProvider.Should().Be("google");
        service.UnlinkUserId.Should().Be(currentUserId);
    }

    private sealed class CapturingAuthenticationService : IAccountAuthenticationService
    {
        public string? BeginProvider { get; private set; }
        public string? BeginRedirectUri { get; private set; }
        public Guid? BeginUserId { get; private set; }
        public string? CompleteProvider { get; private set; }
        public string? CompleteCode { get; private set; }
        public string? CompleteState { get; private set; }
        public string? CompleteRedirectUri { get; private set; }
        public Guid? CompleteUserId { get; private set; }
        public Guid? LoginsUserId { get; private set; }
        public Guid? UnlinkUserId { get; private set; }
        public string? UnlinkProvider { get; private set; }

        public Task<object?> AuthenticateAsync(string username, string password, string? twoFactorCode = null, string? recoveryCode = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<AuthResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<int> RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<AccountSessionDto>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> RevokeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyCollection<ExternalIdentityProviderDto>> GetExternalProvidersAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ExternalIdentityProviderDto> providers =
            [
                new ExternalIdentityProviderDto { Provider = "google", DisplayName = "Google" }
            ];

            return Task.FromResult(providers);
        }

        public Task<ExternalAuthenticationStartDto?> BeginExternalAuthenticationAsync(
            string provider,
            string redirectUri,
            Guid? userId,
            CancellationToken cancellationToken = default)
        {
            BeginProvider = provider;
            BeginRedirectUri = redirectUri;
            BeginUserId = userId;

            return Task.FromResult<ExternalAuthenticationStartDto?>(
                new ExternalAuthenticationStartDto
                {
                    Provider = provider,
                    DisplayName = "Google",
                    AuthorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth",
                    ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(15)
                });
        }

        public Task<ExternalAuthenticationResultDto?> CompleteExternalAuthenticationAsync(
            string provider,
            string code,
            string state,
            string redirectUri,
            Guid? userId,
            CancellationToken cancellationToken = default)
        {
            CompleteProvider = provider;
            CompleteCode = code;
            CompleteState = state;
            CompleteRedirectUri = redirectUri;
            CompleteUserId = userId;

            return Task.FromResult<ExternalAuthenticationResultDto?>(
                new ExternalAuthenticationResultDto
                {
                    Provider = provider,
                    DisplayName = "Google",
                    Email = "john@example.com",
                    IsLinked = true,
                    IsCreated = false,
                    Tokens = new AuthResult
                    {
                        TokenType = "Bearer",
                        AccessToken = "access",
                        ExpiresIn = 900,
                        RefreshToken = "refresh"
                    }
                });
        }

        public Task<IReadOnlyCollection<ExternalLoginDto>> GetExternalLoginsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            LoginsUserId = userId;

            IReadOnlyCollection<ExternalLoginDto> logins =
            [
                new ExternalLoginDto { Provider = "google", DisplayName = "Google" }
            ];

            return Task.FromResult(logins);
        }

        public Task<IdentityResult> UnlinkExternalLoginAsync(
            Guid userId,
            string provider,
            CancellationToken cancellationToken = default)
        {
            UnlinkUserId = userId;
            UnlinkProvider = provider;

            return Task.FromResult(IdentityResult.Success());
        }
    }

    private sealed class StubCurrentUserService : ICurrentUserService
    {
        public StubCurrentUserService(Guid userId) => UserId = userId;

        public Guid UserId { get; }
    }
}
