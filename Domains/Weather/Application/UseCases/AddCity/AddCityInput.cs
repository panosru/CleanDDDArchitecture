namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    using Aviant.DDD.Application.UseCases;

    public sealed class AddCityInput : UseCaseInput
    {
        public AddCityInput(string city) => City = city;

        internal string City { get; }
    }
}
