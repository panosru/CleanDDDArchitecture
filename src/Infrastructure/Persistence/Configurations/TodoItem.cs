namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoItem : IEntityTypeConfiguration<Domain.Entities.TodoItemEntity>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.TodoItemEntity> builder)
        {
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}