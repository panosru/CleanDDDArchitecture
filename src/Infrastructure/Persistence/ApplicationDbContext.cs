namespace CleanArchitecture.Infrastructure.Persistence
{
    using System;
    using Domain.Entities;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Common.Interfaces;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Domain.Entity;
    using Aviant.DDD.Infrastructure;
    using ApplicationUser = Identity.ApplicationUser;
    using ApplicationRole = Identity.ApplicationRole;
    using IDateTime = Aviant.DDD.Application.IDateTime;

    public class ApplicationDbContext : 
        ApiAuthorizationDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IDateTime dateTime) : base(options, operationalStoreOptions)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public DbSet<TodoList> TodoLists { get; set; }

        public DbSet<TodoItem> TodoItems { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<Auditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.Created = _dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModified = _dateTime.Now;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.DeletedBy = _currentUserService.UserId;
                        entry.Entity.Deleted = _dateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            // builder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "User"); });
            // builder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "Role"); });

            base.OnModelCreating(builder);
        }
    }
}
