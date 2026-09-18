using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;

namespace CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;

/// <summary>
///     Delivers an integration event to the contexts that handle it. Called by the outbox
///     dispatcher, never directly by a use case: use cases write to the outbox, in the same
///     transaction as their state change.
/// </summary>
public interface IIntegrationEventPublisher
{
    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
