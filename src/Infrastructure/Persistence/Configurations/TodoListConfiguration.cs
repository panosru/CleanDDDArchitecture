namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Aviant.DDD.Infrastructure.Persistance.Configurations;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoListConfiguration : EntityConfigurationBase<TodoListEntity, int>
    {
        public override void Configure(EntityTypeBuilder<TodoListEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}