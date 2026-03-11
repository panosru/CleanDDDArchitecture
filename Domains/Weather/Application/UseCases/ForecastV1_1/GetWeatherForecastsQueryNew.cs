using Aviant.Application.Queries;
using Aviant.Core.Timing;
using Polly;
using CleanDDDArchitecture.Domains.Weather.Application.Services;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1;

internal sealed record GetWeatherForecastsQueryNew : Query<IEnumerable<WeatherForecastService>>
{
    #region Nested type: GetWeatherForecastsQueryNewHandler

    internal sealed class
        GetWeatherForecastsQueryNewHandler
        : QueryHandler<GetWeatherForecastsQueryNew, IEnumerable<WeatherForecastService>>
    {
        private static readonly string[] Summaries =
        {
            "Freezing v1.1", "Bracing v1.1", "Chilly v1.1", "Cool v1.1", "Mild v1.1",
            "Warm v1.1", "Balmy v1.1", "Hot v1.1", "Sweltering v1.1", "Scorching v1.1"
        };

        private readonly IWeatherForecastCollectionService _weatherForecastCollectionService;

        /// <inheritdoc />
        public GetWeatherForecastsQueryNewHandler(IWeatherForecastCollectionService weatherForecastCollectionService) =>
            _weatherForecastCollectionService = weatherForecastCollectionService;

        public override Task<IEnumerable<WeatherForecastService>> Handle(
            GetWeatherForecastsQueryNew request,
            CancellationToken           cancellationToken)
        {
            return Task.FromResult<IEnumerable<WeatherForecastService>>(
                _weatherForecastCollectionService.BuildForecasts(
                    Clock.Now,
                    Summaries));
        }

        public override IAsyncPolicy RetryPolicy() =>
            Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(
                    2,
                    i => TimeSpan.FromSeconds(i));
    }

    #endregion
}
