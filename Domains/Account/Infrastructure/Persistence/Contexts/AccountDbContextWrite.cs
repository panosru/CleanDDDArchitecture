namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

using Application.Identity;
using Application.Persistence;
using Aviant.Infrastructure.Identity.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

public sealed class AccountDbContextWrite
    : AuthorizationDbContextWrite<AccountDbContextWrite, AccountUser, AccountRole>, IAccountDbContextWrite
{
    public AccountDbContextWrite(
        DbContextOptions<AccountDbContextWrite> options)
        : base(options)
    { }
}
