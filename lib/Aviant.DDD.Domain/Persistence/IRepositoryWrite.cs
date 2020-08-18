namespace Aviant.DDD.Domain.Persistence
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Entities;

    public interface IRepositoryWrite<TEntity, in TPrimaryKey> : IDisposable
        where TEntity : EntityBase<TPrimaryKey>
    {
        Task Add(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(TEntity entity);

        Task DeleteWhere(Expression<Func<TEntity, bool>> predicate);
    }
}