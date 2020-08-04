namespace Aviant.DDD.Infrastructure.Persistance.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Application.Identity;
    using Domain.Entity;
    using Domain.Persistence;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseReadOnly<TDbContext, TApplicationUser, TApplicationRole, TEntity, TPrimaryKey> : 
        IRepositoryRead<TEntity, TPrimaryKey>
        where TEntity : Base<TPrimaryKey>
        where TApplicationUser : ApplicationUser
        where TApplicationRole : ApplicationRole
        where TDbContext : ApplicationDbContextBase<TDbContext, TApplicationUser, TApplicationRole>
    {
        private readonly TDbContext _dbContext;
        private DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        public BaseReadOnly(TDbContext context)
        {
            _dbContext = context;
        }
        
        public IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = Table;
            return query;
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAll();
            BindIncludeProperties(query, includeProperties);
            includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query;
        }

        public async Task<List<TEntity>> GetAllList()
        {
            return await GetAll().ToListAsync();
        }

        public Task<List<TEntity>> GetAllListIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAll();
            includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.ToListAsync();
        }

        public ValueTask<TEntity> Find(TPrimaryKey id)
        {
            return Table.FindAsync(id);
        }

        public Task<TEntity> GetFirst(TPrimaryKey id)
        {
            return GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public Task<TEntity> GetFirstIncluding(TPrimaryKey id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAll();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> GetFirstIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAll();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> GetSingle(TPrimaryKey id)
        {
            return GetAll().SingleOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public Task<TEntity> GetSingleIncluding(TPrimaryKey id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAll();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.SingleOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleOrDefaultAsync(predicate);
        }

        public Task<TEntity> GetSingleIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAll();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.SingleOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate);
        }

        public IQueryable<TEntity> FindByIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAll();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.Where(predicate);
        }

        public Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.AnyAsync(predicate);
        }

        public Task<bool> All(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.AllAsync(predicate);
        }

        public async Task<int> Count()
        {
            return await Table.CountAsync();
        }

        public Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.CountAsync(predicate);
        }

        private static void BindIncludeProperties(IQueryable<TEntity> query, IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
        {
            includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        private static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}