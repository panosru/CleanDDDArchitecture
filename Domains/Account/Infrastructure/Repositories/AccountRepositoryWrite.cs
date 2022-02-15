namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories;

using Application.Aggregates;
using Application.Identity;
using Application.Repositories;
using Aviant.Infrastructure.Identity.Persistence.Repository;
using Persistence.Contexts;

public sealed class AccountRepositoryWrite //TODO: Maybe should add methods for easy creation?
    : RepositoryWrite<AccountDbContextWrite, AccountUser, AccountRole, AccountAggregate, AccountAggregateId>,
      IAccountRepositoryWrite
{
    public AccountRepositoryWrite(AccountDbContextWrite context)
        : base(context)
    { }
}
