namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using MediatR;

    public sealed class SyncWeatherServiceCommand : ICommand
    {
        public string City { get; set; }
    }

    public class SyncWeatherServiceCommandHandler
        : CommandHandler<SyncWeatherServiceCommand>
    {
        public override async Task<Unit> Handle(SyncWeatherServiceCommand command, CancellationToken cancellationToken)
        {
            // Perform some operations here
            if (false)
                throw new Exception("Something went wrong!");

            await Task.Delay(3000, cancellationToken)
               .ConfigureAwait(false);
            
            return Unit.Value;
        }
    }
}