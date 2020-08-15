namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoItemWriteRepository //TODO: Add MarkComplete & UnmarkComplete methods?
        : RepositoryWriteOnlyBase<
                ApplicationDbContext,
                ApplicationUser,
                ApplicationRole,
                TodoItemEntity,
                int>,
            ITodoItemWriteRepository
    {
        public TodoItemWriteRepository(ApplicationDbContext context)
            :
            base(context)
        {
        }
    }
}