namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

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
}