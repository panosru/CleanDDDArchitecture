namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories
{
    using Aviant.DDD.Domain.Persistence;
    using Todo.Core.Entities;

    public interface ITodoListRepositoryWrite : IRepositoryWrite<TodoListEntity, int>
    {}
}