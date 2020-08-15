﻿namespace CleanDDDArchitecture.Infrastructure.Persistence.Configurations
{
    using Aviant.DDD.Infrastructure.Persistance.Configurations;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TodoItemConfiguration : EntityConfigurationBase<TodoItemEntity, int>
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
}