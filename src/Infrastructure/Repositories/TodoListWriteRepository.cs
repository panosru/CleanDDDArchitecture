namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;
    using Persistence.Contexts;

    public class TodoListWriteRepository
        : RepositoryWriteOnlyBase<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoListEntity, int>,
            ITodoListWriteRepository
    {
        public TodoListWriteRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}