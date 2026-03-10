using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.Identity;

public interface IAccountAuthenticationService
{
    public Task<object?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    public Task<AuthResult?> RefreshAsync(
        string refreshToken,
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
}
