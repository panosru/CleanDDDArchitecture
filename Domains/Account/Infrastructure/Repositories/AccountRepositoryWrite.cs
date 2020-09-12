namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories
{
    #region

    using Application.Aggregates;
    using Application.Identity;
    using Application.Repositories;
    using Aviant.DDD.Infrastructure.Persistence.Repository;
    using Identity;
    using Persistence.Contexts;

    #endregion

    public class AccountRepositoryWrite //TODO: Maybe should add methods for easy creation?
        : RepositoryWrite<AccountDbContextWrite, AccountUser, AccountRole, AccountAggregate, AccountAggregateId>,
          IAccountRepositoryWrite
    {
        public AccountRepositoryWrite(AccountDbContextWrite context)
            : base(context)
        { }
    }
}