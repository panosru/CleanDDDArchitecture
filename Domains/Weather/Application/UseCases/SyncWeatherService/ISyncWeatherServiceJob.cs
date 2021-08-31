namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Jobs;
    using Hangfire;

    public sealed class SyncWeatherServiceJobOptions : IJobOptions
    { }

    [Queue("second")]
    public class SyncWeatherServiceJob : IJob<SyncWeatherServiceJobOptions>
    {
        /// <inheritdoc />
        public async Task Perform(SyncWeatherServiceJobOptions jobOptions)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            Console.WriteLine("Weather service syncronised!!");
        }
    }
}