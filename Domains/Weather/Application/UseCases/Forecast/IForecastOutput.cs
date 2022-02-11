namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast;

using Aviant.Application.UseCases;

public interface IForecastOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void Ok(object? @object);
}