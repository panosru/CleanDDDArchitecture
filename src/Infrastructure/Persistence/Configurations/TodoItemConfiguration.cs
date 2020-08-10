namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItemEntity>
    {
        public void Configure(EntityTypeBuilder<TodoItemEntity> builder)
        {
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}