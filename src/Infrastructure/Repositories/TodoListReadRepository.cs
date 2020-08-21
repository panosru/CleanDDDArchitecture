namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class TodoListReadRepository
        : RepositoryReadOnly<TodoDbContextReadOnly, TodoUser, TodoRole, TodoListEntity, int>,
            ITodoListReadRepository
    {
        public TodoListReadRepository(TodoDbContextReadOnly context)
            : base(context)
        {
        }
    }
}