using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast;

public interface IForecastOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void Ok(object? @object);
}
