namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using MediatR;

    internal sealed class SyncWeatherServiceCommand : ICommand
    { }

    internal sealed class SyncWeatherServiceCommandHandler
        : CommandHandler<SyncWeatherServiceCommand>
    {
        private Random Random { get; } = new Random();

        public override async Task<Unit> Handle(SyncWeatherServiceCommand command, CancellationToken cancellationToken)
        {
            // Perform some operations here
            await Task.Delay(3000, cancellationToken)
               .ConfigureAwait(false);

            // 40% probability to fail
            if (Random.Next(100) <= 40)
                throw new Exception("Something gone really wrong...");

            Console.WriteLine("Success message");

            return Unit.Value;
        }
    }
}