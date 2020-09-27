namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class SyncWeatherServiceUseCase
        : UseCase<SyncWeatherServiceInput, ISyncWeatherServiceOutput>
    {
        public override async Task ExecuteAsync(
            SyncWeatherServiceInput input,
            CancellationToken       cancellationToken = default
            )
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                new SyncWeatherServiceCommand
                {
                    City = input.City
                },
                cancellationToken)
               .ConfigureAwait(false);
            
            if (requestResult.Succeeded)
                Output.NoContent();
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}