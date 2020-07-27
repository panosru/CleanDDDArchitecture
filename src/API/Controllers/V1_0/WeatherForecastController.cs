using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers.V1_0
{
    public class WeatherForecastController : ApiController
    {
        /// <summary>
        /// Get weather
        /// </summary>
        /// <returns>weather</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await Mediator.Send(new GetWeatherForecastsQuery());
        }
    }
}
