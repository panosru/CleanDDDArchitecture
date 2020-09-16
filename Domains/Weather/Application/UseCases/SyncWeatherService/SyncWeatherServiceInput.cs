namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    #region

    using Aviant.DDD.Application.UseCases;

    #endregion

    public class SyncWeatherServiceInput : IUseCaseInput
    {
        public SyncWeatherServiceInput(string city) => City = city;

        public string City { get; }
    }
}