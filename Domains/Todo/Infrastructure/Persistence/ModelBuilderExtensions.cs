namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence;

using Core.Entities;
using Microsoft.EntityFrameworkCore;

internal static class ModelBuilderExtensions
{
    private static readonly DateTime SeedCreatedAtUtc = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(this ModelBuilder modelBuilder)
    {
        var shopping = TodoListEntity.Create("Shopping");
        shopping.Id        = -1;
        shopping.Created   = SeedCreatedAtUtc;
        shopping.CreatedBy = Guid.Empty;

        modelBuilder.Entity<TodoListEntity>().HasData(shopping);

        modelBuilder.Entity<TodoItemEntity>()
           .HasData(
                Item(-1, "Apples"),
                Item(-2, "Milk"),
                Item(-3, "Bread"),
                Item(-4, "Toilet paper"),
                Item(-5, "Pasta"),
                Item(-6, "Tissues"),
                Item(-7, "Tuna"),
                Item(-8, "Water"));
    }

    private static TodoItemEntity Item(int id, string title)
    {
        var item = TodoItemEntity.Create(listId: -1, title);
        item.Id = id;

        return item;
    }
}
