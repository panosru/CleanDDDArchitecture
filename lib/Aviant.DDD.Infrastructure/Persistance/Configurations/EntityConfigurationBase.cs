namespace Aviant.DDD.Infrastructure.Persistance.Configurations
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EntityConfigurationBase<TEntity, T> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity<T>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}