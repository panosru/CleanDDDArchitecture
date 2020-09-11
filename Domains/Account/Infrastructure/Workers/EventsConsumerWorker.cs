namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Workers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Aggregates;
    using Aviant.DDD.Domain.EventBus;
    using Infrastructure;
    using Microsoft.Extensions.Hosting;

    public class EventsConsumerWorker : BackgroundService
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
            await tc;
        }
    }
}