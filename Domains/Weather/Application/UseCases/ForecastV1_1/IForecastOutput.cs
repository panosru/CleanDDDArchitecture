using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1;

public interface IForecastOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void Ok(object? @object);
}
