using Aviant.Application.Jobs;
using Hangfire;
using Microsoft.Extensions.Logging;
using CleanDDDArchitecture.Domains.Shared.Core;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService;

internal sealed class SyncWeatherServiceJobOptions : IJobOptions;

[Queue(JobQueue.Main)]
internal class SyncWeatherServiceJob(ILogger<SyncWeatherServiceJob> logger) : IJob<SyncWeatherServiceJobOptions>
{
    /// <inheritdoc />
    public async Task PerformAsync(SyncWeatherServiceJobOptions jobOptions, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10))
           .ConfigureAwait(false);

        logger.LogInformation("Weather service synchronised");
    }
}
