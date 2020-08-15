namespace CleanDDDArchitecture.Services.v1_0
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.WeatherForecasts.Queries.GetWeatherForecasts;
    using Interfaces;
    using MediatR;

    public class WeatherForecastService : BaseService, IWeatherForecastService
    {
        public WeatherForecastService(IMediator mediator)
            : base(mediator)
        {
        }

        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await Mediator.Send(new GetWeatherForecastsQuery());
        }
    }
}