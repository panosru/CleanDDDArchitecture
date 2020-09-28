// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Weather.Application.Services
{
    using System;

    public sealed class WeatherForecastService
    {
        internal WeatherForecastService(
            DateTime date,
            int      temperatureC,
            string   summary)
        {
            Date         = date;
            TemperatureC = temperatureC;
            Summary      = summary ?? throw new ArgumentNullException(nameof(summary));
        }

        public DateTime Date { get; }

        public int TemperatureC { get; }

        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

        public string Summary { get; }
    }
}