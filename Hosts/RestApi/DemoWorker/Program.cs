namespace DemoWorker;

using Hangfire;
using Hangfire.PostgreSql;
using Lamar;

internal static class Program
{
    static void Main()
    {
        var container = new Container(
            _ => _.IncludeRegistry<CommonRegistry>());

        GlobalConfiguration.Configuration
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UsePostgreSqlStorage(
                "User ID =panosru;Password=35pWV2ZVPVjoK2;Server=localhost;Port=5432;Database=Hangfire;Integrated Security=true;Pooling=true;",
                new PostgreSqlStorageOptions
                {
                    // QueuePollInterval = TimeSpan.Zero
                });

        using var server = new BackgroundJobServer(
            new BackgroundJobServerOptions
            {
                ServerName  = $"{Environment.MachineName}.{Guid.NewGuid().ToString()}",
                WorkerCount = Environment.ProcessorCount * 2,
                Queues      = new[] { "third" }
            });

        GlobalConfiguration.Configuration.UseFilter(new AutomaticRetryAttribute { Attempts = 5 });

        Console.WriteLine("Hangfire Server started. Press any key to exit...");
        Console.ReadKey();
    }
}

internal class WorkerActivator : JobActivator
{
    private readonly IContainer _container;

    public WorkerActivator(IContainer container) => _container = container;

    public override object ActivateJob(Type jobType) => _container.GetInstance(jobType);
}
