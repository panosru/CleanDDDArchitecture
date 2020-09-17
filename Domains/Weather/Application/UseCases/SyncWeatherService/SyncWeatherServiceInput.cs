namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using Aviant.DDD.Application.UseCases;

    public class SyncWeatherServiceInput : IUseCaseInput
    {
        public SyncWeatherServiceInput(string city) => City = city;

        public string City { get; }
    }
}