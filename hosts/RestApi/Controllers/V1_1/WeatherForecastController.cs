namespace CleanArchitecture.RestApi.Controllers.V1_1
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.WeatherForecasts.Queries.GetWeatherForecasts;
    using Microsoft.AspNetCore.Mvc;

    public class WeatherForecastController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await Mediator.Send(new GetWeatherForecastsQueryNew());
        }
    }
}