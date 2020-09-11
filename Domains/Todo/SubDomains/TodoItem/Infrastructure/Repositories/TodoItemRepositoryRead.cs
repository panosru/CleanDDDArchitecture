namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories
{
    #region

    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Core.Repositories;
    using Todo.Core.Entities;
    using Todo.Infrastructure.Persistence.Contexts;

    #endregion

    public class TodoItemRepositoryRead
        : RepositoryRead<TodoDbContextRead, TodoItemEntity, int>,
          ITodoItemRepositoryRead
    {
        public TodoItemRepositoryRead(TodoDbContextRead context)
            : base(context)
        { }
    }
}