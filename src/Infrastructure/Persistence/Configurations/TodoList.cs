namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoList : IEntityTypeConfiguration<Domain.Entities.TodoList>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.TodoList> builder)
        {
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}