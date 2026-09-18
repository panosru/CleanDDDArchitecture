using CleanDDDArchitecture.Domains.Account.Core.Aggregates;
using Aviant.Core.Persistence;

namespace CleanDDDArchitecture.Domains.Account.Application.Repositories;

public interface IAccountRepositoryWrite : IRepositoryWrite<AccountAggregate, AccountAggregateId>;
