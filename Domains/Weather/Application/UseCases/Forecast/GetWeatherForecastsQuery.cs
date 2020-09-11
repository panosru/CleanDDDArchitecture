namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Queries;

    #endregion

    public class GetWeatherForecastsQuery : Query<IEnumerable<WeatherForecast>>
    { }

    public class GetWeatherForecastsQueryHandler
        : QueryHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public override Task<IEnumerable<WeatherForecast>> Handle(
            GetWeatherForecastsQuery request,
            CancellationToken        cancellationToken)
        {
            var rng = new Random();

            IEnumerable<WeatherForecast> vm = Enumerable.Range(1, 5)
               .Select(
                    index => new WeatherForecast
                    {
                        Date         = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary      = Summaries[rng.Next(Summaries.Length)]
                    });

            return Task.FromResult(vm);
        }
    }
}