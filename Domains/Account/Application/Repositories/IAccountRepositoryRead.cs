namespace CleanDDDArchitecture.Domains.Account.Application.Repositories;

using Aggregates;
using Aviant.Core.Persistence;

public interface IAccountRepositoryRead : IRepositoryRead<AccountAggregate, AccountAggregateId>
{ }