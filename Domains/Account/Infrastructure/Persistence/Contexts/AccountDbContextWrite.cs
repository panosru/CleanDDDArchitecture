namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts
{
    using Application.Identity;
    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public sealed class AccountDbContextWrite
        : AuthorizationDbContextWrite<AccountDbContextWrite, AccountUser, AccountRole>, IAccountDbContextWrite
    {
        public AccountDbContextWrite(
            DbContextOptions<AccountDbContextWrite> options,
            IOptions<OperationalStoreOptions>       operationalStoreOptions)
            : base(
                options,
                operationalStoreOptions)
        { }
    }
}