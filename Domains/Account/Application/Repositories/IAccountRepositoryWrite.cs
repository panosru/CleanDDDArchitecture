namespace CleanDDDArchitecture.Domains.Account.Application.Repositories;

using Aggregates;
using Aviant.Foundation.Core.Persistence;

public interface IAccountRepositoryWrite : IRepositoryWrite<AccountAggregate, AccountAggregateId>
{ }
