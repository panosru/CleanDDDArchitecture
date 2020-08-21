namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class TodoItemReadRepository
        : RepositoryReadOnly<TodoDbContextReadOnly, TodoUser, TodoRole, TodoItemEntity, int>,
            ITodoItemReadRepository
    {
        public TodoItemReadRepository(TodoDbContextReadOnly context)
            : base(context)
        {
        }
    }
}