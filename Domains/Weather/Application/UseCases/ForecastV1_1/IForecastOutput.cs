namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1
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