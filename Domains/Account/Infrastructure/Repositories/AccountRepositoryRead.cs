namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories
{
    #region

    using Application.Aggregates;
    using Application.Identity;
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Persistence.Contexts;

    #endregion

    public class AccountRepositoryRead
        : RepositoryRead<AccountDbContextRead, AccountUser, AccountRole, AccountAggregate, AccountAggregateId>,
          IAccountRepositoryRead
    {
        public AccountRepositoryRead(AccountDbContextRead context)
            : base(context)
        { }
    }
}