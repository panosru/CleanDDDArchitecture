namespace Aviant.DDD.Infrastructure.Persistance.Repository
{
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseReadOnly<TEntity> : Base<TEntity> where TEntity : class
    {
        protected BaseReadOnly(DbContext context) : base(context)
        {
        }
    }
}