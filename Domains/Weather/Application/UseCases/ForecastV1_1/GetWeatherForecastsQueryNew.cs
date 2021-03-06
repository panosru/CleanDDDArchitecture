﻿namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Queries;
    using Services;

    internal sealed class GetWeatherForecastsQueryNew : Query<IEnumerable<WeatherForecastService>>
    { }

    internal sealed class
        GetWeatherForecastsQueryNewHandler
        : QueryHandler<GetWeatherForecastsQueryNew, IEnumerable<WeatherForecastService>>
    {
        private static readonly string[] Summaries =
        {
            "Freezing v1.1", "Bracing v1.1", "Chilly v1.1", "Cool v1.1", "Mild v1.1",
            "Warm v1.1", "Balmy v1.1", "Hot v1.1", "Sweltering v1.1", "Scorching v1.1"
        };

        public override Task<IEnumerable<WeatherForecastService>> Handle(
            GetWeatherForecastsQueryNew request,
            CancellationToken           cancellationToken)
        {
            var rng = new Random();

            IEnumerable<WeatherForecastService> vm = Enumerable.Range(1, 5)
               .Select(
                    index => new WeatherForecastService(
                        DateTime.Now.AddDays(index),
                        rng.Next(-20, 55),
                        Summaries[rng.Next(Summaries.Length)]));

            return Task.FromResult(vm);
        }
    }
}