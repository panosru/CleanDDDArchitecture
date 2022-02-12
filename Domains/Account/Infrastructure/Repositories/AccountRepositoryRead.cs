namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories;

using Application.Aggregates;
using Application.Identity;
using Application.Repositories;
using Aviant.Foundation.Infrastructure.Persistence.Repository;
using Persistence.Contexts;

public sealed class AccountRepositoryRead
    : RepositoryRead<AccountDbContextRead, AccountUser, AccountRole, AccountAggregate, AccountAggregateId>,
      IAccountRepositoryRead
{
    public AccountRepositoryRead(AccountDbContextRead context)
        : base(context)
    { }
}
