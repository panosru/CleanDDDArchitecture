using Aviant.Application.Jobs;
using Aviant.Infrastructure.Jobs;
using CleanDDDArchitecture.Domains.Shared.Core;
using Hangfire;
using Hangfire.PostgreSql;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Hangfire service extension
/// </summary>
public static class Hangfire
{
    /// <summary>
    ///  Add Hangfire services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddHangfireServices(
        this IServiceCollection services,
        IConfiguration      configuration)
    {
        services.AddSingleton<IJobRunner, JobRunner>();

        services.AddHangfire(
                config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(
                        options =>
                            options.UseNpgsqlConnection(configuration.GetConnectionString("PGSQLConnection")))
                    .UseFilter(
                        new AutomaticRetryAttribute
                        {
                            Attempts = 5
                        }))
            .AddHangfireServer(
                options =>
                {
                    options.ServerName  = $"{Environment.MachineName}.{Guid.NewGuid().ToString()}";
                    options.WorkerCount = Environment.ProcessorCount * 3;
                    options.Queues      = [JobQueue.Main, JobQueue.Second, JobQueue.Default];
                });

        return services;
    }
}
