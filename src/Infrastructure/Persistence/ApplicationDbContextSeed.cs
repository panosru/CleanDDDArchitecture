﻿namespace CleanDDDArchitecture.Infrastructure.Persistence
{
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    public static class ApplicationDbContextSeed
    {
        private static UserManager<ApplicationUser> UserManager;
        
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            
            var defaultUser = new ApplicationUser {UserName = "administrator", Email = "administrator@localhost"};

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
                await userManager.CreateAsync(defaultUser, "Administrator1!");
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            bool modified = false;
            
            // Seed, if necessary
            if (!context.TodoLists.Any())
            {
                context.TodoLists.Add(new TodoListEntity
                {
                    Title = "Shopping",
                    Items =
                    {
                        new TodoItemEntity {Title = "Apples"},
                        new TodoItemEntity {Title = "Milk"},
                        new TodoItemEntity {Title = "Bread"},
                        new TodoItemEntity {Title = "Toilet paper"},
                        new TodoItemEntity {Title = "Pasta"},
                        new TodoItemEntity {Title = "Tissues"},
                        new TodoItemEntity {Title = "Tuna"},
                        new TodoItemEntity {Title = "Water"}
                    }
                });

                modified = true;
            }

            if (!context.Members.Any())
            {
                context.Members.Add(new AccountEntity
                {
                    UserId = UserManager.Users.First(u => "administrator" == u.UserName)
                        .Id
                });

                modified = true;
            }
            
            if (modified)
                await context.SaveChangesAsync();
        }
    }
}