using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1;

public sealed class ForecastUseCase : UseCase<IForecastOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                new GetWeatherForecastsQueryNew(),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
