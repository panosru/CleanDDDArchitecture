namespace Aviant.DDD.Infrastructure.Persistance
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Identity;
    using Application.Persistance;
    using Application.Services;
    using Domain.Entities;
    using IdentityServer4.EntityFramework.Options;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.Extensions.Options;

    public abstract class ApplicationDbContextBase<TDbContext, TApplicationUser, TApplicationRole>
        : ApiAuthorizationDbContext<TApplicationUser, TApplicationRole, Guid>, IApplicationDbContext
        where TDbContext : IApplicationDbContext
        where TApplicationUser : ApplicationUserBase
        where TApplicationRole : ApplicationRoleBase
    {
        private static readonly MethodInfo? ConfigureGlobalFiltersMethodInfo = typeof(TDbContext)
            .GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public ApplicationDbContextBase(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService)
            : base(options, operationalStoreOptions)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;

            ChangeTracker.LazyLoadingEnabled = false;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditedEntity>())
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreationAuditProperties(entry);
                        break;
                    case EntityState.Modified:
                        SetModificationAuditProperties(entry);
                        break;
                    case EntityState.Deleted:
                        CancelDeletionForSoftDelete(entry);
                        SetDeletionAuditProperties(entry);
                        break;
                }
        
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        
            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                ConfigureGlobalFiltersMethodInfo?
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] {modelBuilder, entityType});
        }

        #region Configure Global Filters

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null) modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
            where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity))) return true;

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete) e).IsDeleted;
                expression = softDeleteFilter;
            }

            return expression;
        }

        #endregion

        #region Configure Audit Properties

        protected virtual void SetCreationAuditProperties(EntityEntry entry)
        {
            if (!(entry.Entity is IHasCreationTime hasCreationTimeEntity)) return;

            if (hasCreationTimeEntity.Created == default) hasCreationTimeEntity.Created = _dateTimeService.Now;

            if (!(entry.Entity is ICreationAudited creationAuditedEntity)) return;

            if (creationAuditedEntity.CreatedBy != Guid.Empty)
                //CreatedUserId is already set
                return;

            creationAuditedEntity.CreatedBy = _currentUserService.UserId;
        }

        protected virtual void SetModificationAuditProperties(EntityEntry entry)
        {
            if (!(entry.Entity is IHasModificationTime hasModificationTimeEntity)) return;

            hasModificationTimeEntity.LastModified = _dateTimeService.Now;

            if (!(entry.Entity is IModificationAudited modificationAuditedEntity)) return;

            if (modificationAuditedEntity.LastModifiedBy == _currentUserService.UserId)
                //LastModifiedUserId is same as current user id
                return;

            modificationAuditedEntity.LastModifiedBy = _currentUserService.UserId;
        }

        protected virtual void SetDeletionAuditProperties(EntityEntry entry)
        {
            if (!(entry.Entity is IHasDeletionTime hasDeletionTimeEntity)) return;

            if (hasDeletionTimeEntity.Deleted == default) hasDeletionTimeEntity.Deleted = _dateTimeService.Now;

            if (!(entry.Entity is IDeletionAudited deletionAuditedEntity)) return;

            deletionAuditedEntity.DeletedBy = _currentUserService.UserId;
            deletionAuditedEntity.Deleted = _dateTimeService.Now;
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete)) return;

            entry.Reload();
            entry.State = EntityState.Modified;
            ((ISoftDelete) entry.Entity).IsDeleted = true;
        }

        #endregion
    }
}