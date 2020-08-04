namespace Aviant.DDD.Infrastructure.Persistance.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Identity;
    using Domain.Entity;
    using Domain.Persistence;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseWriteOnly<TDbContext, TApplicationUser, TApplicationRole, TEntity, TPrimaryKey> : 
        IRepositoryWrite<TEntity, TPrimaryKey>
        where TEntity : Base<TPrimaryKey>
        where TApplicationUser : ApplicationUser
        where TApplicationRole : ApplicationRole
        where TDbContext : ApplicationDbContextBase<TDbContext, TApplicationUser, TApplicationRole>
    {
        private readonly TDbContext _dbContext;
        private DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        public BaseWriteOnly(TDbContext context)
        {
            _dbContext = context;
        }
        
        public async Task Add(TEntity entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task Delete(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteWhere(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> entities = Table.Where(predicate);

            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
        }

        public async Task Commit(CancellationToken cancellationToken = new CancellationToken())
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}