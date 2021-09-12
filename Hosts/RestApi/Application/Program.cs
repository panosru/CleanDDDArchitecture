namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Core.Timing;
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Domains.Account.CrossCutting;
    using Domains.Todo.CrossCutting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.CommandLine;
    using Microsoft.Extensions.Configuration.EnvironmentVariables;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Debugging;
    using Console = Colorful.Console;

    /// <summary>
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// </summary>
        private Program()
        { }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Web Host", Color.Green);
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
                        Log.Error(ex, "An error occurred while migrating or seeding the database");

                        throw;
                    }
                }

                SelfLog.Enable(Console.Error);

                await host.RunAsync()
                   .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.ConfigureAppConfiguration(SetupConfiguration);

                        webBuilder.UseSerilog(
                            (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
                        webBuilder.ConfigureServices(SetupServices);
                        webBuilder.UseStartup<Startup>();
                    });

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

            MoveEnvironmentVariablesToEnd(configurationBuilder);

            DependencyInjectionRegistry.SetConfiguration(configurationBuilder);
        }

        /// <summary>
        /// </summary>
        /// <param name="hostBuilderContext"></param>
        /// <param name="services"></param>
        private static void SetupServices(
            WebHostBuilderContext hostBuilderContext,
            IServiceCollection    services)
        {
            Clock.Provider = ClockProviders.Utc;
        }

        /// <summary>
        /// </summary>
        /// <param name="configurationBuilder"></param>
        private static void MoveEnvironmentVariablesToEnd(IConfigurationBuilder configurationBuilder)
        {
            // Items that needs to be moved to the end (FIFO)
            List<IConfigurationSource> fifo = new()
            {
                // Get EnvironmentVariablesConfigurationSource item
                configurationBuilder.Sources
                   .First(
                        i =>
                            i.GetType() == typeof(EnvironmentVariablesConfigurationSource)),

                // Get CommandLineConfigurationSource item
                configurationBuilder.Sources
                   .First(
                        i =>
                            i.GetType() == typeof(CommandLineConfigurationSource))
            };

            fifo.ForEach(
                item =>
                {
                    // Move to the end
                    configurationBuilder.Sources.Remove(item);
                    configurationBuilder.Sources.Add(item);
                });
        }
    }
}