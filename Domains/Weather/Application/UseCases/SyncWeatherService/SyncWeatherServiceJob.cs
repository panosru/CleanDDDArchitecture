namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService;

using Aviant.Application.Jobs;
using Hangfire;
using Serilog;
using Shared.Core;

internal sealed class SyncWeatherServiceJobOptions : IJobOptions
{ }

[Queue(JobQueue.Main)]
internal class SyncWeatherServiceJob : IJob<SyncWeatherServiceJobOptions>
{
    /// <inheritdoc />
    public async Task PerformAsync(SyncWeatherServiceJobOptions jobOptions)
    {
        await Task.Delay(TimeSpan.FromSeconds(10))
           .ConfigureAwait(false);

        Log.Information("Weather service syncronised!!");
    }
}
