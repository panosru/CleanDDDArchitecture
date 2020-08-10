namespace CleanDDDArchitecture.RestApi.Controllers.V1_1
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.WeatherForecasts.Queries.GetWeatherForecasts;
    using Microsoft.AspNetCore.Mvc;
    using WeatherForecastsQuery = Application.WeatherForecasts.Queries.GetWeatherForecasts.WeatherForecast;

    /// <summary>
    /// </summary>
    public class WeatherForecast : ApiController
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<WeatherForecastsQuery>> Get()
        {
            return await Mediator.Send(new GetWeatherForecastsQueryNew());
        }
    }
}