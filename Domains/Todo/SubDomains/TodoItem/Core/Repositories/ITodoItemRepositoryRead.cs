namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories
{
    #region

    using Aviant.DDD.Core.Persistence;
    using Todo.Core.Entities;

    #endregion

    public interface ITodoItemRepositoryRead : IRepositoryRead<TodoItemEntity, int>
    { }
}