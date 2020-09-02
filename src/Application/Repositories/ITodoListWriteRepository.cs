namespace CleanDDDArchitecture.Application.Repositories
{
    using Aviant.DDD.Domain.Persistence;
    using Domain.Entities;

    public interface ITodoListWriteRepository : IRepositoryWrite<TodoListEntity, int>
    { }
}