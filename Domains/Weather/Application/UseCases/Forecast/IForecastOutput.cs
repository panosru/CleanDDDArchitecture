namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast
{
    #region

    using Aviant.DDD.Application.UseCases;

    #endregion

    public interface IForecastOutput : IUseCaseOutput
    {
        public void Invalid(string message);

        public void Ok(object? @object);
    }
}