namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

using Application.Identity;
using Application.Persistence;
using Aviant.Infrastructure.Identity.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

public sealed class AccountDbContextRead
    : AuthorizationDbContextRead<AccountUser, AccountRole>, IAccountDbContextRead
{
    public AccountDbContextRead(
        DbContextOptions<AccountDbContextRead> options)
        : base(options)
    { }
}
