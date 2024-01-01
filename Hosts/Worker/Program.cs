using CleanDDDArchitecture.Hosts.Worker;
using Hangfire;
using Hangfire.SqlServer;
using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;

try
{
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                          ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var config = new ConfigurationBuilder()
        .AddYamlFile("appsettings.yaml", false, true)
        .AddYamlFile($"appsettings.{environment}.yaml", true, true)
        .Build();
    
    // Create a new Container
    var container = new Container(registry => registry.IncludeRegistry<CommonRegistry>());
    
    // // Resolve IBackgroundJobClient
    // var jobClient = container.GetInstance<IBackgroundJobClient>();
    
    // Configure Hangfire with global settings
    GlobalConfiguration.Configuration
        // Set the data compatibility level
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        // Use simple assembly name type serializer
        .UseSimpleAssemblyNameTypeSerializer()
        // Use recommended serializer settings
        .UseRecommendedSerializerSettings()
        // Use SQL Server storage with specific options
        .UseSqlServerStorage(config.GetConnectionString("Hangfire"), 
            new SqlServerStorageOptions
            {
                // QueuePollInterval = TimeSpan.Zero
            });

    // Add Hangfire server with specific options
    using var server = new BackgroundJobServer(
        new BackgroundJobServerOptions
        {
            ServerName = $"{Environment.MachineName}.{Guid.NewGuid().ToString()}",
            WorkerCount = Environment.ProcessorCount * 2,
            Queues = ["third"]
        });

    // Use filter for automatic retries with 5 attempts
    GlobalConfiguration.Configuration.UseFilter(new AutomaticRetryAttribute { Attempts = 5 });

    // Use the job client to create a background job
    // jobClient.Enqueue(() => Console.WriteLine("Hello, Hangfire!"));

    // Display a message on the console indicating that the Hangfire Server has started
    Console.WriteLine("Hangfire Server started. Press any key to exit...");

    // Wait for a key press to exit the application
    Console.ReadKey();
}
catch (Exception ex)
{
    // Log any fatal exception that occurs and print it on the console
    Console.WriteLine(ex);
    Console.WriteLine("Host terminated unexpectedly");
}
