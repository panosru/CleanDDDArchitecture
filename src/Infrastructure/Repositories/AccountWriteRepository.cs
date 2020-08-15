namespace CleanDDDArchitecture.Infrastructure.Repositories
{
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistance.Repository;
    using Domain.Entities;
    using Identity;
    using Persistence;

    public class AccountWriteRepository //TODO: Maybe should add methods for easy creation?
        : RepositoryWriteOnlyBase<ApplicationDbContext, ApplicationUser, ApplicationRole, AccountEntity, int>,
            IAccountWriteRepository
    {
        public AccountWriteRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}