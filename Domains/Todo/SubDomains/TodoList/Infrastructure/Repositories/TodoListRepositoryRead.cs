using Aviant.Infrastructure.Persistence.Repository;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories;

public sealed class TodoListRepositoryRead
    : RepositoryRead<TodoDbContextRead, TodoListEntity, int>,
      ITodoListRepositoryRead
{
    public TodoListRepositoryRead(TodoDbContextRead context)
        : base(context)
    { }
}
