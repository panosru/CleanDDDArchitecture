using Aviant.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Persistence.Configurations;

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
