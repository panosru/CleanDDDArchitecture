namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.Controllers.V1_1
{
    #region

    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Weather.Application.UseCases.Forecast;

    #endregion

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class WeatherForecast : ApiController
    {
        /// <summary>
        ///     Get weather
        /// </summary>
        /// <returns>weather</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RequestResult), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get()
        {
            RequestResult requestResult = await Orchestrator.SendQuery(new GetWeatherForecastsQueryNew());

            if (!requestResult.Succeeded)
                return BadRequest(requestResult.Messages);

            return Ok(requestResult.Payload());
        }
    }
}