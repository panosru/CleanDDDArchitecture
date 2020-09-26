namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class SyncWeatherServiceUseCase
        : UseCase<SyncWeatherServiceInput, ISyncWeatherServiceOutput>
    {
        public override async Task Execute(SyncWeatherServiceInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new SyncWeatherServiceCommand
                {
                    City = input.City
                });
            
            if (requestResult.Succeeded)
                Output.NoContent();
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}