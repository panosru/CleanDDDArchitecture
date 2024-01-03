using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Repositories;
using Aviant.Infrastructure.Identity.Persistence.Repository;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories;

public sealed class AccountRepositoryRead
    : RepositoryRead<AccountDbContextRead, AccountUser, AccountRole, AccountAggregate, AccountAggregateId>,
      IAccountRepositoryRead
{
    public AccountRepositoryRead(AccountDbContextRead context)
        : base(context)
    { }
}
