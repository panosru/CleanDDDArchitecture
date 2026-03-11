using Aviant.Core.Timing;

namespace CleanDDDArchitecture.Domains.Weather.Application.Services;

public interface IWeatherForecastCollectionService
{
    IReadOnlyList<WeatherForecastService> BuildForecasts(
        DateTime referenceDate,
        IReadOnlyList<string> summaries);
}

public sealed class WeatherForecastCollectionService : IWeatherForecastCollectionService
{
    private readonly IWeatherForecastService _weatherForecastService;

    public WeatherForecastCollectionService(IWeatherForecastService weatherForecastService) =>
        _weatherForecastService = weatherForecastService;

    public IReadOnlyList<WeatherForecastService> BuildForecasts(
        DateTime referenceDate,
        IReadOnlyList<string> summaries)
    {
        ArgumentNullException.ThrowIfNull(summaries);

        if (summaries.Count == 0)
            throw new ArgumentException("At least one summary must be provided.", nameof(summaries));

        var startDate = referenceDate == default ? Clock.Now : referenceDate;

        return Enumerable.Range(1, 5)
            .Select(index =>
            {
                var forecastDate = startDate.Date.AddDays(index);
                var summaryIndex = Math.Abs(HashCode.Combine(forecastDate.DayOfYear, index, summaries.Count)) % summaries.Count;
                var temperatureSeed = Math.Abs(HashCode.Combine(forecastDate.Year, forecastDate.DayOfYear, summaryIndex));
                var temperatureC = (temperatureSeed % 75) - 20;

                return _weatherForecastService.GetWeatherForecast(
                    forecastDate,
                    temperatureC,
                    summaries[summaryIndex]);
            })
            .ToArray();
    }
}
