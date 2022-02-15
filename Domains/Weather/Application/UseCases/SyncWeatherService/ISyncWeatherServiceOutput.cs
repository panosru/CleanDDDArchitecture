namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService;

using Aviant.Application.UseCases;

public interface ISyncWeatherServiceOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void NoContent();
}
