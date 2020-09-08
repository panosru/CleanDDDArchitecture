namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories
{
    using Application.Aggregates;
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Identity;
    using Persistence.Contexts;
    
    public class AccountRepositoryRead
        : RepositoryReadOnly<AccountDbContextRead, AccountUser, AccountRole, AccountEntity, AccountId>,
            IAccountRepositoryRead
    {
        public AccountRepositoryRead(AccountDbContextRead context)
            : base(context)
        { }
    }
}