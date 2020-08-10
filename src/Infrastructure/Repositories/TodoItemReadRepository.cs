namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoItemReadRepository
        : RepositoryReadOnlyBase<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoItemEntity, int>,
            ITodoItemReadRepository
    {
        public TodoItemReadRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}