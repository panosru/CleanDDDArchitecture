namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories
{
    using Application.Aggregates;
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Identity;
    using Persistence.Contexts;

    public class AccountRepositoryWrite //TODO: Maybe should add methods for easy creation?
        : RepositoryWriteOnly<AccountDbContextWrite, AccountUser, AccountRole, AccountEntity, AccountId>,
            IAccountRepositoryWrite
    {
        public AccountRepositoryWrite(AccountDbContextWrite context)
            : base(context)
        { }
    }
}