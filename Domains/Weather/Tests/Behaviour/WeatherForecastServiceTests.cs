using CleanDDDArchitecture.Domains.Weather.Application.Services;
using FluentAssertions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Weather.Tests.Behaviour;

public sealed class WeatherForecastServiceTests
{
    [Fact]
    public void GetWeatherForecast_ShouldCreateANewForecastInstance()
    {
        var service = new WeatherForecastService();

        var first = service.GetWeatherForecast(new DateTime(2026, 3, 9), 12, "Warm");
        var second = service.GetWeatherForecast(new DateTime(2026, 3, 10), 18, "Hot");

        first.Should().NotBeSameAs(second);
        first.Date.Should().Be(new DateTime(2026, 3, 9));
        first.TemperatureC.Should().Be(12);
        first.Summary.Should().Be("Warm");
        second.Date.Should().Be(new DateTime(2026, 3, 10));
        second.TemperatureC.Should().Be(18);
        second.Summary.Should().Be("Hot");
    }
}
