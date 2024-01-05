using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.UseCases.V1_0.AddCity;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[FeatureGate(Features.WeatherAddCity)]
public sealed class Weather
    : ApiController<AddCityUseCase, Weather>,
      IAddCityOutput
{
    /// <inheritdoc />
    public Weather([FromServices] AddCityUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IAddCityOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IAddCityOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="city"></param>
    void IAddCityOutput.Ok(string city) =>
        ViewModel = Ok(new AddCityResponse(city).ToString());

    /// <summary>
    /// </summary>
    void IAddCityOutput.NotFound() =>
        ViewModel = NotFound();

    #endregion

    /// <summary>
    ///     Adds an additional city for weather forecasting
    /// </summary>
    /// <response code="200">City added successfully</response>
    /// <response code="201">City added successfully</response>
    /// <response code="400">Bad request.</response>
    /// <param name="dto"></param>
    /// <returns>The name of the added city</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddCityResponse))]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> AddCity([FromBody] AddCityDto dto)
    {
        await UseCase.ExecuteAsync(new AddCityInput(dto.City))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
