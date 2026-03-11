using CleanDDDArchitecture.Domains.Weather.Application.Services;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Weather.Tests.Behaviour;

public sealed class WeatherForecastCollectionServiceTests
{
    [Fact]
    public void BuildForecastsShouldBeDeterministicForTheSameReferenceDate()
    {
        var forecastService = new WeatherForecastService();
        var collectionService = new WeatherForecastCollectionService(forecastService);
        var referenceDate = new DateTime(2026, 3, 11, 0, 0, 0, DateTimeKind.Utc);
        string[] summaries = ["Freezing", "Warm", "Scorching"];

        var first = collectionService.BuildForecasts(referenceDate, summaries);
        var second = collectionService.BuildForecasts(referenceDate, summaries);

        first.Select(item => (item.Date, item.TemperatureC, item.Summary))
            .Should()
            .Equal(second.Select(item => (item.Date, item.TemperatureC, item.Summary)));
    }

    [Fact]
    public void BuildForecastsShouldGenerateFiveSequentialForecasts()
    {
        var forecastService = new WeatherForecastService();
        var collectionService = new WeatherForecastCollectionService(forecastService);
        var referenceDate = new DateTime(2026, 3, 11, 0, 0, 0, DateTimeKind.Utc);

        var forecasts = collectionService.BuildForecasts(referenceDate, ["Cool", "Warm"]);

        forecasts.Should().HaveCount(5);
        forecasts.Select(forecast => forecast.Date)
            .Should()
            .Equal(
                referenceDate.Date.AddDays(1),
                referenceDate.Date.AddDays(2),
                referenceDate.Date.AddDays(3),
                referenceDate.Date.AddDays(4),
                referenceDate.Date.AddDays(5));
        forecasts.Should().OnlyContain(forecast => forecast.TemperatureC >= -20 && forecast.TemperatureC <= 54);
        forecasts.Select(forecast => forecast.Summary)
            .Should()
            .OnlyContain(summary => summary == "Cool" || summary == "Warm");
    }
}
