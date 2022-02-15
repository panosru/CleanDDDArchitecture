namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;

using Aviant.Application.UseCases;

public sealed record AddCityInput(string City) : UseCaseInput
{
    internal string City { get; } = City;
}
