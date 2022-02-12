namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories;

using Aviant.Foundation.Infrastructure.Persistence.Repository;
using Core.Repositories;
using Todo.Core.Entities;
using Todo.Infrastructure.Persistence.Contexts;

public sealed class TodoItemRepositoryWrite //TODO: Add MarkComplete & UnmarkComplete methods?
    : RepositoryWrite<TodoDbContextWrite, TodoItemEntity, int>, ITodoItemRepositoryWrite
{
    public TodoItemRepositoryWrite(TodoDbContextWrite context)
        : base(context)
    { }
}
