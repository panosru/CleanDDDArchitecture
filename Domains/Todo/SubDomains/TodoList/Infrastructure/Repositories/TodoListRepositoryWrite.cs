namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories;

using Aviant.Infrastructure.Persistence.Repository;
using Core.Repositories;
using Todo.Core.Entities;
using Todo.Infrastructure.Persistence.Contexts;

public sealed class TodoListRepositoryWrite
    : RepositoryWrite<TodoDbContextWrite, TodoListEntity, int>,
      ITodoListRepositoryWrite
{
    public TodoListRepositoryWrite(TodoDbContextWrite context)
        : base(context)
    { }
}