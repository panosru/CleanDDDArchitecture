using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts
{
    public class GetWeatherForecastsQueryNew : IRequest<IEnumerable<WeatherForecast>>
    {
    }

    public class GetWeatherForecastsQueryNewHandler : IRequestHandler<GetWeatherForecastsQueryNew, IEnumerable<WeatherForecast>>
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing v1.1", "Bracing v1.1", "Chilly v1.1", "Cool v1.1", "Mild v1.1", 
            "Warm v1.1", "Balmy v1.1", "Hot v1.1", "Sweltering v1.1", "Scorching v1.1"
        };

        public Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQueryNew request, CancellationToken cancellationToken)
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
