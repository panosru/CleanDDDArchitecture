namespace CleanDDDArchitecture.Domains.Account.Application.Repositories
{
    using Aggregates;
    using Aviant.DDD.Domain.Persistence;

    public interface IAccountRepositoryRead : IRepositoryRead<AccountAggregate, AccountAggregateId>
    { }
}