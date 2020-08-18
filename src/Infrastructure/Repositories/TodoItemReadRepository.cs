namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class TodoItemReadRepository
        : RepositoryReadOnlyBase<ApplicationDbContextReadOnly, ApplicationUser, ApplicationRole, TodoItemEntity, int>,
            ITodoItemReadRepository
    {
        public TodoItemReadRepository(ApplicationDbContextReadOnly context)
            : base(context)
        {
        }
    }
}