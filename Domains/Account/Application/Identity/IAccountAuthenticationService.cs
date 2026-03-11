using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.Identity;

public interface IAccountAuthenticationService
{
    public Task<object?> AuthenticateAsync(
        string username,
        string password,
        string? twoFactorCode = null,
        string? recoveryCode = null,
        string? trustedDeviceToken = null,
        bool rememberDevice = false,
        string? deviceName = null,
        CancellationToken cancellationToken = default);

    public Task<AuthResult?> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<ExternalIdentityProviderDto>> GetExternalProvidersAsync(
        CancellationToken cancellationToken = default);

    public Task<ExternalAuthenticationStartDto?> BeginExternalAuthenticationAsync(
        string provider,
        string redirectUri,
        Guid? userId,
        CancellationToken cancellationToken = default);

    public Task<ExternalAuthenticationResultDto?> CompleteExternalAuthenticationAsync(
        string provider,
        string code,
        string state,
        string redirectUri,
        Guid? userId,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<ExternalLoginDto>> GetExternalLoginsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    public Task<Aviant.Application.Identity.IdentityResult> UnlinkExternalLoginAsync(
        Guid userId,
        string provider,
        CancellationToken cancellationToken = default);

    public Task<bool> RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    public Task<int> RevokeAllRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<AccountSessionDto>> GetActiveSessionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    public Task<bool> RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<AccountTrustedDeviceDto>> GetTrustedDevicesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    public Task<bool> RevokeTrustedDeviceAsync(
        Guid userId,
        Guid trustedDeviceId,
        CancellationToken cancellationToken = default);
}
