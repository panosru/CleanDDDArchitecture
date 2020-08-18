namespace CleanDDDArchitecture.Infrastructure.Persistence.Contexts
{
    using Application.Persistence;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Infrastructure.Persistance.Contexts;
    using Domain.Entities;
    using Identity;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class ApplicationDbContext
        : ApplicationDbContextBase<ApplicationDbContext, ApplicationUser, ApplicationRole>, IApplicationDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService)
            : base(
                options,
                operationalStoreOptions,
                currentUserService,
                dateTimeService)
        {
        }

        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TodoItemEntity> TodoItems { get; set; }

        public DbSet<AccountEntity> Accounts { get; set; }
    }
}