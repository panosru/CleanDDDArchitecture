using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Core.Events;
using Polly;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmail.Events;

internal sealed class AccountEmailChangedDomainEventConsumer : DomainEventHandler<AccountEmailChangedDomainEvent>
{
    public override Task Handle(
        EventReceived<AccountEmailChangedDomainEvent> @event,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public override IAsyncPolicy RetryPolicy() =>
        Policy
            .Handle<ArgumentOutOfRangeException>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(i));
}
