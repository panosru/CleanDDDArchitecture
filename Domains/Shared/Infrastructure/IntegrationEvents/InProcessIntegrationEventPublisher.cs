using CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using MediatR;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;

/// <summary>
///     Delivers integration events to handlers in this process, as
///     <see cref="IntegrationEventReceived{TEvent}" /> notifications. Used by the monolith, and
///     by the Kafka consumer once an event has arrived.
/// </summary>
public sealed class InProcessIntegrationEventPublisher(IPublisher publisher) : IIntegrationEventPublisher
{
    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        var notification = (INotification)Activator.CreateInstance(
            typeof(IntegrationEventReceived<>).MakeGenericType(integrationEvent.GetType()),
            integrationEvent)!;

        return publisher.Publish(notification, cancellationToken);
    }
}
