using System.Text;
using CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using Confluent.Kafka;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;

/// <summary>Publishes integration events to a Kafka topic, keyed by event id.</summary>
public sealed class KafkaIntegrationEventPublisher : IIntegrationEventPublisher, IDisposable
{
    internal const string TypeHeader = "event-type";

    private readonly IProducer<string, string> _producer;

    private readonly string _topic;

    public KafkaIntegrationEventPublisher(string bootstrapServers, string topic)
    {
        _topic    = topic;
        _producer = new ProducerBuilder<string, string>(
                new ProducerConfig { BootstrapServers = bootstrapServers, EnableIdempotence = true, Acks = Acks.All })
           .Build();
    }

    public async Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        await _producer.ProduceAsync(
                _topic,
                new Message<string, string>
                {
                    Key     = integrationEvent.Id.ToString(),
                    Value   = IntegrationEventSerializer.Serialize(integrationEvent),
                    Headers = [new Header(TypeHeader, Encoding.UTF8.GetBytes(IntegrationEventSerializer.TypeName(integrationEvent)))]
                },
                cancellationToken)
           .ConfigureAwait(false);
    }

    public void Dispose() => _producer.Dispose();
}
