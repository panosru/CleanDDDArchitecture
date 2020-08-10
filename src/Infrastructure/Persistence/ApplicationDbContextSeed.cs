namespace CleanDDDArchitecture.Infrastructure.Persistence
{
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser {UserName = "administrator", Email = "administrator@localhost"};

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
                await userManager.CreateAsync(defaultUser, "Administrator1!");
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.TodoLists.Any())
            {
                context.TodoLists.Add(new TodoListEntity
                {
                    Title = "Shopping",
                    Items =
                    {
                        new TodoItemEntity {Title = "Apples", Done = true},
                        new TodoItemEntity {Title = "Milk", Done = true},
                        new TodoItemEntity {Title = "Bread", Done = true},
                        new TodoItemEntity {Title = "Toilet paper"},
                        new TodoItemEntity {Title = "Pasta"},
                        new TodoItemEntity {Title = "Tissues"},
                        new TodoItemEntity {Title = "Tuna"},
                        new TodoItemEntity {Title = "Water"}
                    }
                });

                await context.SaveChangesAsync();
            }
        }
    }
}