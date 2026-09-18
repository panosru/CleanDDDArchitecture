using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Core.Events;
using Polly;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail.Events;

internal sealed class AccountEmailConfirmedDomainEventConsumer : DomainEventHandler<AccountEmailConfirmedDomainEvent>
{
    public override Task Handle(
        EventReceived<AccountEmailConfirmedDomainEvent> @event,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public override IAsyncPolicy RetryPolicy() =>
        Policy
            .Handle<ArgumentOutOfRangeException>()
            .WaitAndRetryAsync(
                3,
                i => TimeSpan.FromSeconds(i));
}
