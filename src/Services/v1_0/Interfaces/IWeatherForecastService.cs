using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Services;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.Services.v1_0.Interfaces
{
    public interface IWeatherForecastService : IService
    {
        public Task<IEnumerable<WeatherForecast>> Get();
    }
}