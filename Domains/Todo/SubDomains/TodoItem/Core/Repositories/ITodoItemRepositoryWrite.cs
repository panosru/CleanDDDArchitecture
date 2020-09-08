namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories
{
    using Aviant.DDD.Domain.Persistence;
    using CleanDDDArchitecture.Domains.Todo.Core.Entities;

    public interface ITodoItemRepositoryWrite : IRepositoryWrite<TodoItemEntity, int>
    {}
}