// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Weather.Application.Services;

using Aviant.Application.Interceptors;
using Interceptors;

public interface IWeatherForecastService
{
    WeatherForecastService GetWeatherForecast(
        DateTime date,
        int      temperatureC,
        string   summary);
}

[Interceptor<WeatherInterceptor>(Explicit = true)]
public sealed class WeatherForecastService : IWeatherForecastService
{
    public DateTime Date { get; private set; }

    public int TemperatureC { get; private set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; private set; }

    /// <inheritdoc />
    [Intercept]
    public WeatherForecastService GetWeatherForecast(
        DateTime date,
        int      temperatureC,
        string   summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary ?? throw new ArgumentNullException(nameof(summary));

        return this;
    }
}
