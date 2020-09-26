namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.AddCity
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Weather.Application.UseCases.AddCity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    [AllowAnonymous]
    [FeatureGate(Features.WeatherAddCity)]
    public class Weather
        : ApiController<AddCityUseCase, Weather>,
          IAddCityOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public Weather([FromServices] AddCityUseCase useCase)
            : base(useCase)
        { }

        #region IAddCityOutput Members

        void IAddCityOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IAddCityOutput.Ok(string city) =>
            ViewModel = Ok(new AddCityResponse(city).ToString());

        void IAddCityOutput.NotFound() =>
            ViewModel = NotFound();

        #endregion

        /// <summary>
        ///     Adds an additional city for weather forecasting
        /// </summary>
        /// <response code="201">City added successfully</response>
        /// <response code="400">Bad request.</response>
        /// <param name="dto"></param>
        /// <returns>The name of the added city</returns>
        [HttpPost("[action]")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
        public async Task<IActionResult> AddCity([FromBody] AddCityDto dto)
        {
            await UseCase
               .SetInput(dto)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}