namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class TodoListReadRepository :
        RepositoryReadOnlyBase<ApplicationDbContext, ApplicationUser, ApplicationRole, TodoListEntity, int>, ITodoListReadRepository
    {
        public TodoListReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}