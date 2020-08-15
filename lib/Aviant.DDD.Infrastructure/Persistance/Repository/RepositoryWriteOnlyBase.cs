namespace Aviant.DDD.Infrastructure.Persistance.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Identity;
    using Domain.Entities;
    using Domain.Persistence;
    using Microsoft.EntityFrameworkCore;

    public abstract class RepositoryWriteOnlyBase<TDbContext, TApplicationUser, TApplicationRole, TEntity, TPrimaryKey>
        : IRepositoryWrite<TEntity, TPrimaryKey>
        where TEntity : EntityBase<TPrimaryKey>
        where TApplicationUser : ApplicationUserBase
        where TApplicationRole : ApplicationRoleBase
        where TDbContext : ApplicationDbContextBase<TDbContext, TApplicationUser, TApplicationRole>
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<TEntity> _table;

        protected RepositoryWriteOnlyBase(TDbContext context)
        {
            _dbContext = context;
            _table = _dbContext.Set<TEntity>();
        }

        public async Task Add(TEntity entity)
        {
            await _table.AddAsync(entity);
        }

        public Task Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            
            return Task.CompletedTask;
        }

        public Task Delete(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            
            return Task.CompletedTask;
        }

        public Task DeleteWhere(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> entities = _table.Where(predicate);

            foreach (var entity in entities) _dbContext.Entry(entity).State = EntityState.Deleted;
            
            return Task.CompletedTask;
        }

        public async Task Commit(CancellationToken cancellationToken = new CancellationToken())
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            //_dbContext?.Dispose();
        }
    }
}