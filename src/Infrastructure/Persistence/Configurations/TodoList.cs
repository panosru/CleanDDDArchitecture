namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoList : IEntityTypeConfiguration<Domain.Entities.TodoListEntity>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.TodoListEntity> builder)
        {
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}