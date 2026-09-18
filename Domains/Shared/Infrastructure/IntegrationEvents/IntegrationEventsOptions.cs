namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;

public enum IntegrationEventTransport
{
    /// <summary>Handlers run in the same process (the monolith).</summary>
    InProcess,

    /// <summary>Events travel over Kafka between separately deployed services.</summary>
    Kafka
}

/// <summary>Bound from the <c>IntegrationEvents</c> configuration section.</summary>
public sealed class IntegrationEventsOptions
{
    public const string Section = "IntegrationEvents";

    public IntegrationEventTransport Transport { get; set; } = IntegrationEventTransport.InProcess;

    public string Topic { get; set; } = "cleanddd.integration-events";

    /// <summary>Kafka consumer group; each service needs its own to receive every event.</summary>
    public string? ConsumerGroup { get; set; }

    public TimeSpan OutboxPollingInterval { get; set; } = TimeSpan.FromSeconds(2);

    public int OutboxBatchSize { get; set; } = 50;
}
