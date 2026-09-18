using Aviant.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories;

public sealed class TodoListRepositoryWrite
    : RepositoryWrite<TodoDbContextWrite, TodoListEntity, int>,
      ITodoListRepositoryWrite,
      ITodoListOwnerCleanup
{
    public TodoListRepositoryWrite(TodoDbContextWrite context)
        : base(context)
    { }

    public async Task<int> SoftDeleteOwnedByAsync(
        Guid              ownerId,
        DateTime          deletedAtUtc,
        CancellationToken cancellationToken = default)
    {
        // Set-based updates: no entities are loaded, and nothing goes through SaveChanges, whose
        // auditing needs a current HTTP user that a background event handler does not have.
        IQueryable<int> ownedLists = DbContext.TodoLists
           .Where(list => list.CreatedBy == ownerId && !list.IsDeleted)
           .Select(list => list.Id);

        await DbContext.TodoItems
           .Where(item => ownedLists.Contains(item.ListId) && !item.IsDeleted)
           .ExecuteUpdateAsync(
                set => set.SetProperty(item => item.IsDeleted, true).SetProperty(item => item.Deleted, deletedAtUtc),
                cancellationToken)
           .ConfigureAwait(false);

        return await DbContext.TodoLists
           .Where(list => list.CreatedBy == ownerId && !list.IsDeleted)
           .ExecuteUpdateAsync(
                set => set.SetProperty(list => list.IsDeleted, true).SetProperty(list => list.Deleted, deletedAtUtc),
                cancellationToken)
           .ConfigureAwait(false);
    }
}
