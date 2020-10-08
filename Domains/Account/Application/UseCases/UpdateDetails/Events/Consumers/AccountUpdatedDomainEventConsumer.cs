namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Core.DomainEvents;
    using Aviant.DDD.Core.EventBus;
    using Polly;

    internal sealed class AccountUpdatedDomainEventConsumer : DomainEventHandler<AccountUpdatedDomainEvent>
    {
        public override Task Handle(
            EventReceived<AccountUpdatedDomainEvent> @event,
            CancellationToken                        cancellationToken) =>
            throw new ArgumentOutOfRangeException();

        public override IAsyncPolicy RetryPolicy() =>
            Policy
               .Handle<ArgumentOutOfRangeException>()
               .WaitAndRetryAsync(
                    3,
                    i => TimeSpan.FromSeconds(i));
    }
}