using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.UseCases.V1_0.Forecast;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.WeatherForecast)]
[AllowAnonymous]
public sealed class Weather
    : ApiController<ForecastUseCase, Weather>,
      IForecastOutput
{
    /// <inheritdoc />
    public Weather([FromServices] ForecastUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IForecastOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IForecastOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="object"></param>
    void IForecastOutput.Ok(object? @object) =>
        ViewModel = Ok(@object);

    #endregion

    /// <summary>
    ///     Performs weather forecast
    /// </summary>
    /// <response code="200">Weather forecast</response>
    /// <response code="404">Not found.</response>
    /// <returns>The forecast for the next days</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
    public async Task<IActionResult> Forecast()
    {
        await UseCase.ExecuteAsync()
           .ConfigureAwait(false);

        return ViewModel;
    }
}
