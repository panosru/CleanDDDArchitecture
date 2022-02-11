namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1;

using Aviant.Application.UseCases;

public interface IForecastOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void Ok(object? @object);
}