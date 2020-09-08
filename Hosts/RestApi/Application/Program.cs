namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    using System;
    using System.Threading.Tasks;
    // using Domains.Todo.Infrastructure.Identity;
    // using Domains.Todo.Infrastructure.Persistence.Contexts;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

    /// <summary>
    /// </summary>
    public class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // try
                // {
                //     var context = services.GetRequiredService<TodoDbContextWrite>();
                //
                //     if (context.Database.IsNpgsql()) await context.Database.MigrateAsync();
                //
                //     var userManager = services.GetRequiredService<UserManager<TodoUser>>();
                //
                //     await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager);
                //     await ApplicationDbContextSeed.SeedSampleDataAsync(context);
                // }
                // catch (Exception ex)
                // {
                //     var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                //
                //     logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                //
                //     throw;
                // }
            }

            await host.RunAsync();
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(
                            webBuilder =>
                            {
                                webBuilder.UseStartup<Startup>();
                                webBuilder.ConfigureAppConfiguration(SetupConfiguration);
                                webBuilder.ConfigureLogging(SetupLogging);
                            });
        }

        private static void SetupConfiguration(
            WebHostBuilderContext hostBuilderContext,
            IConfigurationBuilder configurationBuilder)
        {
            var configuration = configurationBuilder.Build();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
        }

        private static void SetupLogging(WebHostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddSerilog();
        }
    }
}