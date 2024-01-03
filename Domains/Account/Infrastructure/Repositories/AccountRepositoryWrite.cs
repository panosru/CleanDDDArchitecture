using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Repositories;
using Aviant.Infrastructure.Identity.Persistence.Repository;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories;

public sealed class AccountRepositoryWrite //TODO: Maybe should add methods for easy creation?
    : RepositoryWrite<AccountDbContextWrite, AccountUser, AccountRole, AccountAggregate, AccountAggregateId>,
      IAccountRepositoryWrite
{
    public AccountRepositoryWrite(AccountDbContextWrite context)
        : base(context)
    { }
}
