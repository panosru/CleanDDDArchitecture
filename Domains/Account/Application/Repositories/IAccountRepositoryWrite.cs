namespace CleanDDDArchitecture.Domains.Account.Application.Repositories
{
    #region

    using Aggregates;
    using Aviant.DDD.Core.Persistence;

    #endregion

    public interface IAccountRepositoryWrite : IRepositoryWrite<AccountAggregate, AccountAggregateId>
    { }
}