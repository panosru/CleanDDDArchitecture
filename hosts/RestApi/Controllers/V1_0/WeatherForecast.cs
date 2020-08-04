namespace CleanDDDArchitecture.RestApi.Controllers.V1_0
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WeatherForecastQuery = Application.WeatherForecasts.Queries.GetWeatherForecasts.WeatherForecast;
    using CleanDDDArchitecture.Services.v1_0.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    public class WeatherForecast : ApiController, IWeatherForecastService
    {
        private readonly IWeatherForecastService _weatherForecastService;

        /// <summary>
        /// </summary>
        public WeatherForecast(IWeatherForecastService weatherForecast)
        {
            _weatherForecastService = weatherForecast;
        }

        /// <summary>
        ///     Get weather
        /// </summary>
        /// <returns>weather</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<WeatherForecastQuery>> Get()
        {
            return await _weatherForecastService.Get();
        }
    }
}