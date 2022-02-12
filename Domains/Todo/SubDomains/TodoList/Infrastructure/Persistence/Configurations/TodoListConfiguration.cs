namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Persistence.Configurations;

using Aviant.Foundation.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Core.Entities;

public sealed class TodoListConfiguration : EntityConfiguration<TodoListEntity, int>
{
    public override void Configure(EntityTypeBuilder<TodoListEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(t => t.Title)
           .HasMaxLength(200)
           .IsRequired();
    }
}
