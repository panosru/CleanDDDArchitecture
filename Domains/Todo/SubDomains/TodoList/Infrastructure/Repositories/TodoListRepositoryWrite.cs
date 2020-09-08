namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories
{
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Core.Repositories;
    using Todo.Core.Entities;
    using Todo.Infrastructure.Identity;
    using Todo.Infrastructure.Persistence.Contexts;

    public class TodoListRepositoryWrite
        : RepositoryWriteOnly<TodoDbContextWrite, TodoUser, TodoRole, TodoListEntity, int>,
          ITodoListRepositoryWrite
    {
        public TodoListRepositoryWrite(TodoDbContextWrite context)
            : base(context)
        { }
    }
}