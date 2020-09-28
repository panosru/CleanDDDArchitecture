namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public sealed class SyncWeatherServiceUseCase
        : UseCase<ISyncWeatherServiceOutput>
    {
        public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                    new SyncWeatherServiceCommand(),
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.NoContent();
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}