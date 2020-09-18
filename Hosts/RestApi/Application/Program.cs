namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    using System;
    using System.Threading.Tasks;
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Domains.Account.CrossCutting;
    using Domains.Todo.CrossCutting;
    using Microsoft.AspNetCore.Hosting;
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
                var serviceProvider = scope.ServiceProvider;

                try
                {
                    await AccountCrossCutting.GenerateDefaultUserIfNotExists(serviceProvider);
                    await TodoCrossCutting.GenerateTodoMigrationsIfNewExists(serviceProvider);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                    throw;
                }
            }

            await host.RunAsync();
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
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
            DependencyInjectionRegistry.ConfigurationBuilder = configurationBuilder;

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(DependencyInjectionRegistry.SetConfiguration(configurationBuilder.Build()))
               .CreateLogger();
        }

        private static void SetupLogging(WebHostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddSerilog();
        }
    }
}