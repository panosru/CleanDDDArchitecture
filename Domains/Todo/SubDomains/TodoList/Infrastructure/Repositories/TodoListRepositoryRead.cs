namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories
{
    #region

    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Core.Repositories;
    using Todo.Core.Entities;
    using Todo.Infrastructure.Persistence.Contexts;

    #endregion

    public class TodoListRepositoryRead
        : RepositoryRead<TodoDbContextRead, TodoListEntity, int>,
          ITodoListRepositoryRead
    {
        public TodoListRepositoryRead(TodoDbContextRead context)
            : base(context)
        { }
    }
}