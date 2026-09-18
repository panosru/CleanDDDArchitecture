namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;

/// <summary>
///     Removes what an owner holds when the owning account is deleted.
/// </summary>
public interface ITodoListOwnerCleanup
{
    /// <summary>
    ///     Soft-deletes every list the owner created, with its items, as of
    ///     <paramref name="deletedAtUtc" />. Lists already deleted are left alone, so calling it
    ///     again for the same owner changes nothing.
    /// </summary>
    /// <returns>The number of lists removed.</returns>
    public Task<int> SoftDeleteOwnedByAsync(Guid ownerId, DateTimeOffset deletedAtUtc, CancellationToken cancellationToken = default);
}
