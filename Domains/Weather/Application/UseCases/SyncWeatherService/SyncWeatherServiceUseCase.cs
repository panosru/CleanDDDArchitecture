namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class SyncWeatherServiceUseCase
        : UseCase<SyncWeatherServiceInput, ISyncWeatherServiceOutput>
    {
        private SyncWeatherServiceCommand? _command;

        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                _command ?? throw new NullReferenceException(typeof(SyncWeatherServiceUseCase).FullName));

            if (requestResult.Succeeded)
                Output.NoContent();
            else
                Output.Invalid(requestResult.Messages.First());
        }

        public SyncWeatherServiceUseCase SetInput(SyncWeatherServiceCommand command)
        {
            _command = command;
            Input    = new SyncWeatherServiceInput(command.City);

            return this;
        }
    }
}