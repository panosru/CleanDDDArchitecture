namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories
{
    #region

    using Aviant.DDD.Domain.Persistence;
    using Todo.Core.Entities;

    #endregion

    public interface ITodoItemRepositoryWrite : IRepositoryWrite<TodoItemEntity, int>
    { }
}