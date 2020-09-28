namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    using System;
    using System.IO;
    using System.Linq;
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
    internal sealed class Program
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
                    await AccountCrossCutting.GenerateDefaultUserIfNotExists(serviceProvider)
                       .ConfigureAwait(false);

                    await TodoCrossCutting.GenerateTodoMigrationsIfNewExists(serviceProvider)
                       .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                    throw;
                }
            }

            await host.RunAsync()
               .ConfigureAwait(false);
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
                        webBuilder.ConfigureAppConfiguration(SetupConfiguration);
                        webBuilder.ConfigureLogging(SetupLogging);
                        webBuilder.UseStartup<Startup>();
                    });
        }

        /// <summary>
        /// </summary>
        /// <param name="hostBuilderContext"></param>
        /// <param name="configurationBuilder"></param>
        private static void SetupConfiguration(
            WebHostBuilderContext hostBuilderContext,
            IConfigurationBuilder configurationBuilder)
        {
            DependencyInjectionRegistry.ConfigurationBuilder = configurationBuilder;

            string[] configFiles =
            {
                "appsettings.yml",
                $"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.yml",
                "appsettings.yaml",
                $"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.yaml"
            };

            foreach (var configFile in configFiles
               .Where(
                    configFile =>
                        File.Exists(
                            Path.Combine(Directory.GetCurrentDirectory(), configFile))))
                configurationBuilder.AddYamlFile(configFile, false, true);

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(DependencyInjectionRegistry.SetConfiguration(configurationBuilder))
               .CreateLogger();
        }

        /// <summary>
        /// </summary>
        /// <param name="hostBuilderContext"></param>
        /// <param name="loggingBuilder"></param>
        private static void SetupLogging(WebHostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddSerilog();
        }
    }
}