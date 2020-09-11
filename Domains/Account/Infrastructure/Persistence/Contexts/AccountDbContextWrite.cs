namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts
{
    #region

    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Core.Entities;
    using Identity;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    #endregion

    public class AccountDbContextWrite
        : AuthorizationDbContextWrite<AccountDbContextWrite, AccountUser, AccountRole>, IAccountDbContextWrite
    {
        public AccountDbContextWrite(
            DbContextOptions<AccountDbContextWrite> options,
            IOptions<OperationalStoreOptions>       operationalStoreOptions)
            : base(
                options,
                operationalStoreOptions)
        { }

        #region IAccountDbContextWrite Members

        public DbSet<AccountEntity> Accounts { get; set; }

        #endregion
    }
}