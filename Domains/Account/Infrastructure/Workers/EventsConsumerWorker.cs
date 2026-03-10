using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Core.EventSourcing.EventBus;
using Microsoft.Extensions.Hosting;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Workers;

public sealed class EventsConsumerWorker : BackgroundService
{
    private readonly IEventConsumerFactory _eventConsumerFactory;

    public EventsConsumerWorker(IEventConsumerFactory eventConsumerFactory) =>
        _eventConsumerFactory = eventConsumerFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IEnumerable<IEventConsumer> consumers = new[]
        {
            _eventConsumerFactory.Build<AccountAggregate, AccountAggregateId, AccountIdDeserializer>()
        };

        var tc = Task.WhenAll(consumers.Select(c => c.ConsumeAsync(stoppingToken)));
        await tc.ConfigureAwait(false);
    }
}
