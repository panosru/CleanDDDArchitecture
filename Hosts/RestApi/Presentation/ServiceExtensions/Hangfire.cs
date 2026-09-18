using Aviant.Application.Jobs;
using Aviant.Infrastructure.Jobs;
using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Weather.CrossCutting;
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
    /// <param name="environment"></param>
    /// <returns></returns>
    public static IServiceCollection AddHangfireServices(
        this IServiceCollection services,
        IConfiguration      configuration,
        IHostEnvironment    environment)
    {
        // Registers the runner and every job in the domains, and checks at startup that
        // each can be constructed: Hangfire builds a job only when it runs. A job that
        // cannot be built stops the app in development; in production it is logged and
        // the other jobs keep running.
        services.AddAviantJobs(jobs =>
        {
            jobs.AddAssemblies(
            [
                .. AccountCrossCutting.MediatorAssemblies(),
                .. WeatherCrossCutting.MediatorAssemblies()
            ]);
            jobs.FailOnUnresolvableJobs = environment.IsDevelopment();
        });

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
