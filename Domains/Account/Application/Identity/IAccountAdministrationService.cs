using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.Identity;

public interface IAccountAdministrationService
{
    public Task<IdentityResult> DeactivateAsync(
        Guid userId,
        string currentPassword,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> SuspendAsync(
        Guid actorUserId,
        Guid targetUserId,
        string? reason,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> UnsuspendAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> UnlockAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<string>?> GetRolesAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> ReplaceRolesAsync(
        Guid actorUserId,
        Guid targetUserId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<AccountClaimDto>?> GetClaimsAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> ReplaceClaimsAsync(
        Guid actorUserId,
        Guid targetUserId,
        IEnumerable<AccountClaimDto> claims,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<AccountSecurityEventDto>> GetOwnSecurityEventsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<AccountSecurityEventDto>?> GetSecurityEventsAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<AccountAdminSummaryDto>?> SearchAccountsAsync(
        Guid actorUserId,
        string? query,
        string? status,
        string? role,
        CancellationToken cancellationToken = default);

    public Task<EmailConfirmationTicket?> GenerateEmailConfirmationForUserAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    public Task<PasswordResetTicket?> GeneratePasswordResetForUserAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    public Task<int?> RevokeAllSessionsAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);
}
