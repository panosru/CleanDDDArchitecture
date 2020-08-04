namespace Aviant.DDD.Domain.Persistence
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Entity;

    public interface IRepositoryWrite<TEntity, in TPrimaryKey> : IDisposable 
        where TEntity : Base<TPrimaryKey>
    {
        Task Add(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(TEntity entity);

        Task DeleteWhere(Expression<Func<TEntity, bool>> predicate);

        Task Commit(CancellationToken cancellationToken);
    }
}