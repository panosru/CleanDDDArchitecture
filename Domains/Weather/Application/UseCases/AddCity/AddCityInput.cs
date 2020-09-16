namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    #region

    using Aviant.DDD.Application.UseCases;

    #endregion

    public class AddCityInput : IUseCaseInput
    {
        public AddCityInput(string city) => City = city;

        public string City { get; }
    }
}