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
        public override Task<Unit> Handle(SyncWeatherServiceCommand command, CancellationToken cancellationToken)
        {
            // Perform some operations here
            if (false)
                throw new Exception("Something went wrong!");

            return Task.FromResult(Unit.Value); // Equivalent to void
        }
    }
}