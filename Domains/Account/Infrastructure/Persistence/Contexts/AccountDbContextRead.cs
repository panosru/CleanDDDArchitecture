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

    public class AccountDbContextRead
        : AuthorizationDbContextRead<AccountUser, AccountRole>, IAccountDbContextRead
    {
        public AccountDbContextRead(
            DbContextOptions                  options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        { }

        #region IAccountDbContextRead Members

        public DbSet<AccountEntity> Accounts { get; set; }

        #endregion
    }
}