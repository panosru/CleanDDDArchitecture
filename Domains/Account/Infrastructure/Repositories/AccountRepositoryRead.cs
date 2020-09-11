namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories
{
    using Application.Aggregates;
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Identity;
    using Persistence.Contexts;
    
    public class AccountRepositoryRead
        : RepositoryRead<AccountDbContextRead, AccountUser, AccountRole, AccountAggregate, AccountAggregateId>,
            IAccountRepositoryRead
    {
        public AccountRepositoryRead(AccountDbContextRead context)
            : base(context)
        { }
    }
}