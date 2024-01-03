using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;

public sealed record AddCityInput(string City) : UseCaseInput
{
    internal string City { get; } = City;
}
