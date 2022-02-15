namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;

using Aviant.Application.UseCases;

public interface IAddCityOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void Ok(string city);

    public void NotFound();
}
