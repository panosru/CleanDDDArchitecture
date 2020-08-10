namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class AccountReadRepository
        : RepositoryReadOnlyBase<ApplicationDbContext, ApplicationUser, ApplicationRole, AccountEntity, int>,
            IAccountReadRepository
    {
        public AccountReadRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}