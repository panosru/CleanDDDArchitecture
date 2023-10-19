using CleanDDDArchitecture.Hosts.RestApi.Worker;
using Hangfire;
using Hangfire.PostgreSql;
using Lamar;

try
{
    // Create a new Container
    var container = new Container(registry => registry.IncludeRegistry<CommonRegistry>());

    // Configure Hangfire with global settings
    GlobalConfiguration.Configuration
        // Set the data compatibility level
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        // Use simple assembly name type serializer
        .UseSimpleAssemblyNameTypeSerializer()
        // Use recommended serializer settings
        .UseRecommendedSerializerSettings()
        // Use PostgreSQL storage with specific options
        .UsePostgreSqlStorage(
            "User ID =panosru;Password=rX3t9bvDT2ftH2;Server=localhost;Port=15432;Database=Hangfire;Integrated Security=true;Pooling=false;Maximum Pool Size=500;ConnectionIdleLifetime=5;ConnectionPruningInterval=2",
            new PostgreSqlStorageOptions
            {
                // QueuePollInterval = TimeSpan.Zero
            });

    // Add Hangfire server with specific options
    using var server = new BackgroundJobServer(
        new BackgroundJobServerOptions
        {
            ServerName = $"{Environment.MachineName}.{Guid.NewGuid().ToString()}",
            WorkerCount = Environment.ProcessorCount * 2,
            Queues = new[] { "third" }
        });

    // Use filter for automatic retries with 5 attempts
    GlobalConfiguration.Configuration.UseFilter(new AutomaticRetryAttribute { Attempts = 5 });


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
