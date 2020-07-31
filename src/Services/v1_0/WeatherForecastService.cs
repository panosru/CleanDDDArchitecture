using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Services.v1_0.Interfaces;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using MediatR;

namespace CleanArchitecture.Services.v1_0
{
    public class WeatherForecastService : BaseService, IWeatherForecastService
    {
        public WeatherForecastService(IMediator mediator) 
            : base(mediator)
        {
        }

        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await _mediator.Send(new GetWeatherForecastsQuery());
        }
    }
}