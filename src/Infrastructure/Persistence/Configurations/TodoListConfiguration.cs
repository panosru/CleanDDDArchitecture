namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoListConfiguration : IEntityTypeConfiguration<TodoListEntity>
    {
        public void Configure(EntityTypeBuilder<TodoListEntity> builder)
        {
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}