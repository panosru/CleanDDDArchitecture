namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    using Aviant.DDD.Application.UseCases;

    public class AddCityInput : UseCaseInput
    {
        public AddCityInput(string city) => City = city;

        public string City { get; }
    }
}