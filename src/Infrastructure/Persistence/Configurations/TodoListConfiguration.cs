namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Aviant.DDD.Infrastructure.Persistence.Configurations;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoListConfiguration : EntityConfiguration<TodoListEntity, int>
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