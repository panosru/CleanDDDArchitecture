namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class SyncWeatherServiceUseCase : UseCase<SyncWeatherServiceInput, ISyncWeatherServiceOutput>
    {
        private SyncWeatherServiceCommand _command;

        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(_command);

            if (requestResult.Succeeded)
                Output.NoContent();
            else
                Output.Invalid(requestResult.Messages.First());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            if (!(data is SyncWeatherServiceCommand command))
                throw new TypeAccessException(
                    $"Expected type \"{nameof(SyncWeatherServiceCommand)}\", but \"{data.GetType().Name}\" found instead.");

            Input    = new SyncWeatherServiceInput(command.City);
            _command = command;
        }
    }
}