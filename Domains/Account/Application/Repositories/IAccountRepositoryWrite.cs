namespace CleanDDDArchitecture.Domains.Account.Application.Repositories
{
    #region

    using Aggregates;
    using Aviant.DDD.Domain.Persistence;

    #endregion

    public interface IAccountRepositoryWrite : IRepositoryWrite<AccountAggregate, AccountAggregateId>
    { }
}