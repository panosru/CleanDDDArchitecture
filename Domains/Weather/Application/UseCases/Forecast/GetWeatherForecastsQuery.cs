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

        private readonly IWeatherForecastService _weatherForecastService;

        /// <inheritdoc />
        public GetWeatherForecastsQueryHandler(IWeatherForecastService weatherForecastService) =>
            _weatherForecastService = weatherForecastService;

        public override Task<IEnumerable<WeatherForecastService>> Handle(
            GetWeatherForecastsQuery request,
            CancellationToken        cancellationToken)
        {
            var rng = new Random();

            IEnumerable<WeatherForecastService> vm = Enumerable.Range(1, 5)
               .Select(
                    index => _weatherForecastService.GetWeatherForecast(
                        Clock.Now.AddDays(index),
                        rng.Next(-20, 55),
                        Summaries[rng.Next(Summaries.Length)]));

            return Task.FromResult(vm);
        }
    }

    #endregion
}
