using AwesomeAssertions;
using CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace CleanDDDArchitecture.Domains.Weather.Tests.Behaviour;

/// <summary>
///     The minimal API translates the use case's output port into typed HTTP results.
/// </summary>
public sealed class WeatherEndpointsTests
{
    [Fact]
    public void OkOutputBecomes200WithThePayload()
    {
        var output = new WeatherEndpoints.ForecastResult();

        output.Ok(new[] { "sunny" });

        output.Result.Result.Should().BeOfType<Ok<object>>().Which.Value.Should().BeEquivalentTo(new[] { "sunny" });
    }

    [Fact]
    public void InvalidOutputBecomes400ProblemDetails()
    {
        var output = new WeatherEndpoints.ForecastResult();

        output.Invalid("No forecast for that city.");

        var problem = output.Result.Result.Should().BeOfType<ProblemHttpResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem.ProblemDetails.Detail.Should().Be("No forecast for that city.");
    }

    [Fact]
    public void WithoutOutputTheResultIsAServerError() =>
        new WeatherEndpoints.ForecastResult().Result.Result.Should().BeOfType<ProblemHttpResult>()
           .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
}
