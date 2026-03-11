namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence;

using Core.Entities;
using Microsoft.EntityFrameworkCore;

internal static class ModelBuilderExtensions
{
    private static readonly DateTime SeedCreatedAtUtc = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoListEntity>()
           .HasData(
                new TodoListEntity
                {
                    Id        = -1,
                    Created   = SeedCreatedAtUtc,
                    CreatedBy = Guid.Empty,
                    IsDeleted = false,
                    Title     = "Shopping"
                });

        modelBuilder.Entity<TodoItemEntity>()
           .HasData(
                new TodoItemEntity { Title = "Apples", Id       = -1, ListId = -1 },
                new TodoItemEntity { Title = "Milk", Id         = -2, ListId = -1 },
                new TodoItemEntity { Title = "Bread", Id        = -3, ListId = -1 },
                new TodoItemEntity { Title = "Toilet paper", Id = -4, ListId = -1 },
                new TodoItemEntity { Title = "Pasta", Id        = -5, ListId = -1 },
                new TodoItemEntity { Title = "Tissues", Id      = -6, ListId = -1 },
                new TodoItemEntity { Title = "Tuna", Id         = -7, ListId = -1 },
                new TodoItemEntity { Title = "Water", Id        = -8, ListId = -1 });
    }
}
