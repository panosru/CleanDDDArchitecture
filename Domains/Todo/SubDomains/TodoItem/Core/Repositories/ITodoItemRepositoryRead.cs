namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories
{
    #region

    using Aviant.DDD.Domain.Persistence;
    using Todo.Core.Entities;

    #endregion

    public interface ITodoItemRepositoryRead : IRepositoryRead<TodoItemEntity, int>
    { }
}