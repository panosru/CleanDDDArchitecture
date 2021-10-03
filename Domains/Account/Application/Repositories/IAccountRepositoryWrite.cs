namespace CleanDDDArchitecture.Domains.Account.Application.Repositories
{
    using Aggregates;
    using Aviant.DDD.Core.Persistence;

    public interface IAccountRepositoryWrite : IRepositoryWrite<AccountAggregate, AccountAggregateId>
    { }
}
