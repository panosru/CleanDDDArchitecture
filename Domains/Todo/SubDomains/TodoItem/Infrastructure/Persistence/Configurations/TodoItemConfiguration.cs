using Aviant.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Persistence.Configurations;

public sealed class TodoItemConfiguration : EntityConfiguration<TodoItemEntity, int>
{
    public override void Configure(EntityTypeBuilder<TodoItemEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.List)
           .WithMany(f => f.Items)
           .HasForeignKey(e => e.ListId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.Title)
           .HasMaxLength(200)
           .IsRequired();
    }
}
