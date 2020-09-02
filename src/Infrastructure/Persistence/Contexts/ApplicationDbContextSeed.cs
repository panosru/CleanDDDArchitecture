namespace CleanDDDArchitecture.Infrastructure.Persistence.Contexts
{
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    public static class ApplicationDbContextSeed //TODO: Revisit 
    {
        private static UserManager<TodoUser> _userManager;

        public static async Task SeedDefaultUserAsync(UserManager<TodoUser> userManager)
        {
            _userManager = userManager;

            var defaultUser = new TodoUser { UserName = "administrator", Email = "administrator@localhost" };

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
                await userManager.CreateAsync(defaultUser, "Administrator1!");
        }

        public static async Task SeedSampleDataAsync(TodoDbContext context)
        {
            var modified = false;

            // Seed, if necessary
            if (!context.TodoLists.Any())
            {
                context.TodoLists.Add(
                    new TodoListEntity
                    {
                        Title = "Shopping",
                        Items =
                        {
                            new TodoItemEntity { Title = "Apples" },
                            new TodoItemEntity { Title = "Milk" },
                            new TodoItemEntity { Title = "Bread" },
                            new TodoItemEntity { Title = "Toilet paper" },
                            new TodoItemEntity { Title = "Pasta" },
                            new TodoItemEntity { Title = "Tissues" },
                            new TodoItemEntity { Title = "Tuna" },
                            new TodoItemEntity { Title = "Water" }
                        }
                    });

                modified = true;
            }

            // if (!context.Accounts.Any())
            // {
            //     var currentUser = _userManager.Users.First(u => "administrator" == u.UserName);
            //
            //     context.Accounts.Add(AccountEntity.Create(currentUser.Id, currentUser.Email));
            //
            //     modified = true;
            // }

            if (modified)
                await context.SaveChangesAsync();
        }
    }
}