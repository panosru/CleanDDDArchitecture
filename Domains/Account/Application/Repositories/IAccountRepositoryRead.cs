namespace CleanDDDArchitecture.Domains.Account.Application.Repositories
{
    using Aggregates;
    using Aviant.DDD.Core.Persistence;

    public interface IAccountRepositoryRead : IRepositoryRead<AccountAggregate, AccountAggregateId>
    { }
}
