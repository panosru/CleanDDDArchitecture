using Aviant.Core.Timing;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Configuration
        .AddYamlFile("appsettings.yaml", false, true)
        .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true, true)
        .AddEnvironmentVariables();

    Clock.Provider = ClockProviders.Utc;

    builder.Services
        .AddHangfire(
            configuration =>
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(
                        options =>
                            options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("PGSQLConnection")))
                    .UseFilter(new AutomaticRetryAttribute { Attempts = 5 }))
        .AddHangfireServer(
            options =>
            {
                options.ServerName = $"{Environment.MachineName}.{Guid.NewGuid()}";
                options.WorkerCount = Environment.ProcessorCount * 2;
                options.Queues = ["main", "second", "third", "default"];
            });

    await builder.Build()
        .RunAsync()
        .ConfigureAwait(false);
}
catch (Exception ex)
{
    // Log any fatal exception that occurs and print it on the console
    Console.WriteLine(ex);
    Console.WriteLine("Host terminated unexpectedly");
}
