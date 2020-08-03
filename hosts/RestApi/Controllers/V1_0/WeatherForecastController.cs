namespace CleanArchitecture.RestApi.Controllers.V1_0
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.WeatherForecasts.Queries.GetWeatherForecasts;
    using CleanArchitecture.Services.v1_0.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    public class WeatherForecastController : ApiController, IWeatherForecastService
    {
        private readonly IWeatherForecastService _weatherForecastService;

        /// <summary>
        /// </summary>
        public WeatherForecastController(IWeatherForecastService weatherForecast)
        {
            _weatherForecastService = weatherForecast;
        }

        /// <summary>
        ///     Get weather
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