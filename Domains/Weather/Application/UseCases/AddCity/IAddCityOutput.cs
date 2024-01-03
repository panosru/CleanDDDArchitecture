using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;

public interface IAddCityOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void Ok(string city);

    public void NotFound();
}
