namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories
{
    using Aviant.DDD.Core.Persistence;
    using Todo.Core.Entities;

    public interface ITodoItemRepositoryWrite : IRepositoryWrite<TodoItemEntity, int>
    { }
}