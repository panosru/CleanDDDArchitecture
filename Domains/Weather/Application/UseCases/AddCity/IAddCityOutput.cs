namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    #region

    using Aviant.DDD.Application.UseCases;

    #endregion

    public interface IAddCityOutput : IUseCaseOutput
    {
        public void Invalid(string message);

        public void Ok(object? @object);

        public void NotFound();
    }
}