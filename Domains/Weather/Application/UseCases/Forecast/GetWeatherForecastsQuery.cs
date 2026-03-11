using Aviant.Application.Queries;
using Aviant.Core.Timing;
using CleanDDDArchitecture.Domains.Weather.Application.Services;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast;

internal sealed record GetWeatherForecastsQuery : Query<IEnumerable<WeatherForecastService>>
{
    #region Nested type: GetWeatherForecastsQueryHandler

    internal sealed class GetWeatherForecastsQueryHandler
        : QueryHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecastService>>
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IWeatherForecastCollectionService _weatherForecastCollectionService;

        /// <inheritdoc />
        public GetWeatherForecastsQueryHandler(IWeatherForecastCollectionService weatherForecastCollectionService) =>
            _weatherForecastCollectionService = weatherForecastCollectionService;

        public override Task<IEnumerable<WeatherForecastService>> Handle(
            GetWeatherForecastsQuery request,
            CancellationToken        cancellationToken)
        {
            return Task.FromResult<IEnumerable<WeatherForecastService>>(
                _weatherForecastCollectionService.BuildForecasts(
                    Clock.Now,
                    Summaries));
        }
    }

    #endregion
}
