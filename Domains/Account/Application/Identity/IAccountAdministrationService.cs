using Aviant.Application.Identity;

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
}
