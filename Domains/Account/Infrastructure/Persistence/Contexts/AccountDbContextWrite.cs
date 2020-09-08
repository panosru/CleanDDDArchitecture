namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts
{
    using Application.Aggregates;
    using Application.Persistence;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Identity;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class AccountDbContextWrite
        : ApplicationDbContext<AccountDbContextWrite, AccountUser, AccountRole>, IAccountDbContextWrite
    {
        public AccountDbContextWrite(
            DbContextOptions<AccountDbContextWrite> options,
            IOptions<OperationalStoreOptions>       operationalStoreOptions,
            ICurrentUserService                     currentUserService,
            IDateTimeService                        dateTimeService)
            : base(
                options,
                operationalStoreOptions,
                currentUserService,
                dateTimeService)
        { }

        public DbSet<AccountEntity> Accounts { get; set; }
    }
}