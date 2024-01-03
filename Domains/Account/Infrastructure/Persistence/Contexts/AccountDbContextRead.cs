using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Persistence;
using Aviant.Infrastructure.Identity.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

public sealed class AccountDbContextRead
    : AuthorizationDbContextRead<AccountUser, AccountRole>, IAccountDbContextRead
{
    public AccountDbContextRead(
        DbContextOptions<AccountDbContextRead> options)
        : base(options)
    { }
}
