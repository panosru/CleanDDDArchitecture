namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories
{
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Core.Repositories;
    using Todo.Core.Entities;
    using Todo.Infrastructure.Identity;
    using Todo.Infrastructure.Persistence.Contexts;

    public class TodoListRepositoryRead
        : RepositoryReadOnly<TodoDbContextRead, TodoUser, TodoRole, TodoListEntity, int>,
          ITodoListRepositoryRead
    {
        public TodoListRepositoryRead(TodoDbContextRead context)
            : base(context)
        { }
    }
}