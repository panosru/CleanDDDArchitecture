using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Core.Persistence;

namespace CleanDDDArchitecture.Domains.Account.Application.Repositories;

public interface IAccountRepositoryRead : IRepositoryRead<AccountAggregate, AccountAggregateId>;
