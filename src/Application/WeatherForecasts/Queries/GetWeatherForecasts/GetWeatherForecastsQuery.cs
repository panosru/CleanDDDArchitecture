namespace CleanDDDArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;

    public class GetWeatherForecastsQuery : CommandBase<IEnumerable<WeatherForecast>>
    {
    }

    public class GetWeatherForecastsQueryCommandHandler :
        CommandHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public override Task<IEnumerable<WeatherForecast>> Handle(
            GetWeatherForecastsQuery request, CancellationToken cancellationToken)
        {
            var rng = new Random();

            var vm = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });

            return Task.FromResult(vm);
        }
    }
}