namespace CleanDDDArchitecture.Domains.Account.Application.Repositories
{
    #region

    using Aggregates;
    using Aviant.DDD.Core.Persistence;

    #endregion

    public interface IAccountRepositoryRead : IRepositoryRead<AccountAggregate, AccountAggregateId>
    { }
}