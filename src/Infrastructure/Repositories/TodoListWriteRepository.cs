namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class TodoListWriteRepository
        : RepositoryWriteOnly<TodoDbContext, TodoUser, TodoRole, TodoListEntity, int>,
          ITodoListWriteRepository
    {
        public TodoListWriteRepository(TodoDbContext context)
            : base(context)
        { }
    }
}