namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    #region

    using Aviant.DDD.Application.UseCases;

    #endregion

    public interface ISyncWeatherServiceOutput : IUseCaseOutput
    {
        public void Invalid(string message);

        public void NoContent();
    }
}