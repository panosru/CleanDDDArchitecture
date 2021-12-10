namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService;

using Aviant.DDD.Application.UseCases;

public interface ISyncWeatherServiceOutput : IUseCaseOutput
{
    public void Invalid(string message);

    public void NoContent();
}