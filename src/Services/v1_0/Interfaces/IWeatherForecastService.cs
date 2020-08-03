namespace CleanArchitecture.Services.v1_0.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.WeatherForecasts.Queries.GetWeatherForecasts;

    public interface IWeatherForecastService : IService
    {
        public Task<IEnumerable<WeatherForecast>> Get();
    }
}