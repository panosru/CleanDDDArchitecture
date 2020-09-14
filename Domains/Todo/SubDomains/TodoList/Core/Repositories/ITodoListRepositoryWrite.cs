namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories
{
    #region

    using Aviant.DDD.Core.Persistence;
    using Todo.Core.Entities;

    #endregion

    public interface ITodoListRepositoryWrite : IRepositoryWrite<TodoListEntity, int>
    { }
}