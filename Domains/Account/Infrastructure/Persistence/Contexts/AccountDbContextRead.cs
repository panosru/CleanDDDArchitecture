namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

using Application.Identity;
using Application.Persistence;
using Aviant.Infrastructure.Persistence.Contexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class AccountDbContextRead
    : AuthorizationDbContextRead<AccountUser, AccountRole>, IAccountDbContextRead
{
    public AccountDbContextRead(
        DbContextOptions<AccountDbContextRead> options,
        IOptions<OperationalStoreOptions>      operationalStoreOptions)
        : base(options, operationalStoreOptions)
    { }
}
