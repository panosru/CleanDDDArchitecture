using Aviant.Application.ApplicationEvents;
using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Core.Events;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events;

internal sealed class AccountCreatedDomainEventConsumer(ILogger<AccountCreatedDomainEventConsumer> logger)
    : DomainEventHandler<AccountCreatedDomainEvent>
{
    public override async Task Handle(
        EventReceived<AccountCreatedDomainEvent> @event,
        CancellationToken                        cancellationToken)
    {
        logger.LogInformation(
            "AccountCreatedDomainEvent consumed for aggregate {AggregateId} and email {Email}",
            @event.Event.Id,
            @event.Event.Email);

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
