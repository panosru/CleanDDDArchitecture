namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Queries;
    using Services;

    #endregion

    public class GetWeatherForecastsQuery : Query<IEnumerable<WeatherForecastService>>
    { }

    public class GetWeatherForecastsQueryHandler
        : QueryHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecastService>>
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public override Task<IEnumerable<WeatherForecastService>> Handle(
            GetWeatherForecastsQuery request,
            CancellationToken        cancellationToken)
        {
            var rng = new Random();

            IEnumerable<WeatherForecastService> vm = Enumerable.Range(1, 5)
               .Select(
                    index => new WeatherForecastService
                    {
                        Date         = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary      = Summaries[rng.Next(Summaries.Length)]
                    });

            return Task.FromResult(vm);
        }
    }
}