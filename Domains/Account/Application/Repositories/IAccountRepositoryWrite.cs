namespace CleanDDDArchitecture.Domains.Account.Application.Repositories
{
    using Aggregates;
    using Aviant.DDD.Domain.Persistence;

    public interface IAccountRepositoryWrite : IRepositoryWrite<AccountEntity, AccountId>
    {
    }
}