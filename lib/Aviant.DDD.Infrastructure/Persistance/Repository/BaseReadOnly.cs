using Microsoft.EntityFrameworkCore;

namespace Aviant.DDD.Infrastructure.Persistance.Repository
{
    public abstract class BaseReadOnly<TEntity> : Base<TEntity> where TEntity : class
    {
        protected BaseReadOnly(DbContext context) : base(context)
        {
        }
    }
}