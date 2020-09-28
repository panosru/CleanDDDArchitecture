namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories
{
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Core.Repositories;
    using Todo.Core.Entities;
    using Todo.Infrastructure.Persistence.Contexts;

    public sealed class TodoItemRepositoryRead
        : RepositoryRead<TodoDbContextRead, TodoItemEntity, int>,
          ITodoItemRepositoryRead
    {
        public TodoItemRepositoryRead(TodoDbContextRead context)
            : base(context)
        { }
    }
}