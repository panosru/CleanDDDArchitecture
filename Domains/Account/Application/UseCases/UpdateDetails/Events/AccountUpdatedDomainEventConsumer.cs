using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Core.Events;
using Polly;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events;

internal sealed class AccountUpdatedDomainEventConsumer : DomainEventHandler<AccountUpdatedDomainEvent>
{
    public override Task Handle(
        EventReceived<AccountUpdatedDomainEvent> @event,
        CancellationToken                        cancellationToken) =>
        Task.CompletedTask;

    public override IAsyncPolicy RetryPolicy() =>
        Policy
           .Handle<ArgumentOutOfRangeException>()
           .WaitAndRetryAsync(
                3,
                i => TimeSpan.FromSeconds(i));
}
