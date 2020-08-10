namespace CleanDDDArchitecture.Infrastructure.Persistence
{
    using Application.Persistence;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Infrastructure.Persistance;
    using Domain.Entities;
    using Identity;
    using IdentityServer4.EntityFramework.Options;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class ApplicationDbContext
        : ApplicationDbContextBase<ApplicationDbContext, ApplicationUser, ApplicationRole>, IApplicationDbContext
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IMediator mediator,
            IDateTimeService dateTimeService)
            : base(
                options,
                operationalStoreOptions,
                currentUserService,
                mediator,
                dateTimeService)
        {
        }

        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TodoItemEntity> TodoItems { get; set; }

        public DbSet<AccountEntity> Members { get; set; }
    }
}