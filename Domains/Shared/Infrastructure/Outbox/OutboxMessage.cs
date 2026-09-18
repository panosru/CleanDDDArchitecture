using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.Outbox;

/// <summary>
///     An integration event waiting to be published, stored in the same database and the same
///     transaction as the change that raised it. If the change commits, the event will be
///     published; if it rolls back, the event never existed.
/// </summary>
public sealed class OutboxMessage
{
    public const int MaxAttempts = 10;

    private OutboxMessage()
    { }

    public Guid Id { get; private set; }

    /// <summary>The contract type name, as <see cref="IntegrationEventSerializer" /> writes it.</summary>
    public string Type { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTime OccurredAtUtc { get; private set; }

    public DateTime? ProcessedAtUtc { get; private set; }

    public int Attempts { get; private set; }

    public string? LastError { get; private set; }

    public static OutboxMessage From(IIntegrationEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        return new OutboxMessage
        {
            Id            = integrationEvent.Id,
            Type          = IntegrationEventSerializer.TypeName(integrationEvent),
            Payload       = IntegrationEventSerializer.Serialize(integrationEvent),
            OccurredAtUtc = integrationEvent.OccurredAtUtc
        };
    }
}
