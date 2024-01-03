using Aviant.Infrastructure.Persistence.Repository;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories;

public sealed class TodoItemRepositoryWrite //TODO: Add MarkComplete & UnmarkComplete methods?
    : RepositoryWrite<TodoDbContextWrite, TodoItemEntity, int>, ITodoItemRepositoryWrite
{
    public TodoItemRepositoryWrite(TodoDbContextWrite context)
        : base(context)
    { }
}
