using Aviant.DDD.Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aviant.DDD.Infrastructure.Persistance.Repository
{
    public abstract class Base<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext Context;

        protected DbSet<TEntity> DbSet;

        public Base(DbContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }
    }
}