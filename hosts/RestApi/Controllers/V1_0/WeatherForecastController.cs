using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.API.v1_0;
using CleanArchitecture.API.v1_0.Interfaces;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.REST.Controllers.V1_0
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherForecastController : ApiController, IWeatherForecastService
    {
        private readonly IWeatherForecastService _weatherForecastService;
        
        /// <summary>
        /// 
        /// </summary>
        public WeatherForecastController(IWeatherForecastService weatherForecast)
        {
            _weatherForecastService = weatherForecast;
        }

        /// <summary>
        /// Get weather
        /// </summary>
        /// <returns>weather</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await _weatherForecastService.Get();
        }
    }
}
