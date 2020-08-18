namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;
    using Persistence.Contexts;

    public class TodoListReadRepository
        : RepositoryReadOnlyBase<ApplicationDbContextReadOnly, ApplicationUser, ApplicationRole, TodoListEntity, int>,
            ITodoListReadRepository
    {
        public TodoListReadRepository(ApplicationDbContextReadOnly context)
            : base(context)
        {
        }
    }
}