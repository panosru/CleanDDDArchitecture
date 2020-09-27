namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class ForecastUseCase : UseCase<IForecastOutput>
    {
        public override async Task ExecuteAsync(CancellationToken cancellationToken = default
        )
        {
            RequestResult requestResult = await Orchestrator.SendQueryAsync(
                new GetWeatherForecastsQueryNew(), cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}