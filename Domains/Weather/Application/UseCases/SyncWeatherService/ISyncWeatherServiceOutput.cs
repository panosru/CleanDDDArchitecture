using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService;

public interface ISyncWeatherServiceOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void NoContent();
}
