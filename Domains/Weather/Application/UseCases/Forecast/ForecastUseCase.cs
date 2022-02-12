namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast;

using Aviant.Foundation.Application.Orchestration;
using Aviant.Foundation.Application.UseCases;

public sealed class ForecastUseCase : UseCase<IForecastOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                new GetWeatherForecastsQuery(),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
