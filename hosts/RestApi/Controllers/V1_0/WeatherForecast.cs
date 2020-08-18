namespace CleanDDDArchitecture.RestApi.Controllers.V1_0
{
    using System.Threading.Tasks;
    using Application.WeatherForecasts.Queries.GetWeatherForecasts;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using WeatherForecastQuery = Application.WeatherForecasts.Queries.GetWeatherForecasts.WeatherForecast;

    /// <summary>
    /// </summary>
    public class WeatherForecast : ApiController
    {
        private readonly IOrchestrator _orchestrator;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orchestrator"></param>
        public WeatherForecast(IOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

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
            RequestResult requestResult = await _orchestrator.SendQuery(new GetWeatherForecastsQuery());

            if (!requestResult.Success)
                return BadRequest(requestResult.Messages);
            
            return Ok(requestResult.Payload());
        }
    }
}