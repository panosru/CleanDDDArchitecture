using CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.FeatureManagement;

namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.Endpoints;

/// <summary>
///     The weather forecast as a minimal API, next to the controller in UseCases/V1_0/Forecast.
/// </summary>
/// <remarks>
///     Both call the same <see cref="ForecastUseCase" />: the use case does not know or care
///     which style presents it. See docs/adr/007-controllers-and-minimal-apis.md for when
///     to choose which.
/// </remarks>
public static class WeatherEndpoints
{
    public static IEndpointRouteBuilder MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var weather = app.MapGroup("/api/minimal/weather")
           .WithTags("Weather (minimal API)")
           .AllowAnonymous();

        weather.MapGet("/forecast", GetForecastAsync)
           .WithName("GetWeatherForecastMinimal")
           .WithSummary("Five-day weather forecast")
           .WithDescription("The same use case as GET /api/weather, presented through a minimal API endpoint.");

        return app;
    }

    internal static async Task<Results<Ok<object>, ProblemHttpResult, NotFound>> GetForecastAsync(
        ForecastUseCase   useCase,
        IFeatureManager   features,
        CancellationToken cancellationToken)
    {
        if (!await features.IsEnabledAsync(nameof(Features.WeatherForecast)).ConfigureAwait(false))
            return TypedResults.NotFound();

        var output = new ForecastResult();
        useCase.SetOutput(output);

        await useCase.ExecuteAsync(cancellationToken).ConfigureAwait(false);

        return output.Result;
    }

    /// <summary>Translates the use case's output port into an HTTP result.</summary>
    internal sealed class ForecastResult : IForecastOutput
    {
        public Results<Ok<object>, ProblemHttpResult, NotFound> Result { get; private set; } =
            TypedResults.Problem("The forecast use case produced no result.");

        public void Invalid(string message) =>
            Result = TypedResults.Problem(detail: message, statusCode: StatusCodes.Status400BadRequest);

        public void Ok(object? @object) => Result = TypedResults.Ok(@object ?? Array.Empty<object>());

        public void BadRequest(object? @object) =>
            Result = TypedResults.Problem(detail: @object?.ToString(), statusCode: StatusCodes.Status400BadRequest);
    }
}
