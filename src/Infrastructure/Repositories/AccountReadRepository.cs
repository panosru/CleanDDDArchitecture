namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence.Contexts;

    public class AccountReadRepository
        : RepositoryReadOnlyBase<ApplicationDbContextReadOnly, ApplicationUser, ApplicationRole, AccountEntity, int>,
            IAccountReadRepository
    {
        public AccountReadRepository(ApplicationDbContextReadOnly context)
            : base(context)
        {
        }
    }
}