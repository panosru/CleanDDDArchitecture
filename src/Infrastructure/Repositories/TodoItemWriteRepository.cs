namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class TodoItemWriteRepository //TODO: Add MarkComplete & UnmarkComplete methods?
        : RepositoryWriteOnly<
              TodoDbContext,
              TodoUser,
              TodoRole,
              TodoItemEntity,
              int>,
          ITodoItemWriteRepository
    {
        public TodoItemWriteRepository(TodoDbContext context)
            :
            base(context)
        { }
    }
}