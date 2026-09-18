using Aviant.Application.ApplicationEvents;
using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Core.Events;
using Serilog;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events;

internal sealed class AccountCreatedDomainEventConsumer : DomainEventHandler<AccountCreatedDomainEvent>
{
    public override async Task Handle(
        EventReceived<AccountCreatedDomainEvent> @event,
        CancellationToken                        cancellationToken)
    {
        Log.Information(
            "AccountCreatedDomainEvent consumed for aggregate {AggregateId} and email {Email}",
            @event.Event.Id,
            @event.Event.Email);

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
