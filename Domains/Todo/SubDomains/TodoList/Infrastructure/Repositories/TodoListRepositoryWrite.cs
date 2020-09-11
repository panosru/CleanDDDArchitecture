namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories
{
    #region

    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Core.Repositories;
    using Todo.Core.Entities;
    using Todo.Infrastructure.Persistence.Contexts;

    #endregion

    public class TodoListRepositoryWrite
        : RepositoryWrite<TodoDbContextWrite, TodoListEntity, int>,
          ITodoListRepositoryWrite
    {
        public TodoListRepositoryWrite(TodoDbContextWrite context)
            : base(context)
        { }
    }
}