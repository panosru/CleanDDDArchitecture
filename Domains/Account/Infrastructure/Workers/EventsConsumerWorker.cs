namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Workers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Aggregates;
    using Aviant.DDD.Core.EventBus;
    using Microsoft.Extensions.Hosting;

    public sealed class EventsConsumerWorker : BackgroundService
    {
        private readonly IEventConsumerFactory _eventConsumerFactory;

        public EventsConsumerWorker(IEventConsumerFactory eventConsumerFactory) =>
            _eventConsumerFactory = eventConsumerFactory;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            IEnumerable<IEventConsumer> consumers = new[]
            {
                _eventConsumerFactory.Build<AccountAggregate, AccountAggregateId, AccountIdDeserializer>()
            };

            var tc = Task.WhenAll(consumers.Select(c => c.ConsumeAsync(cancellationToken)));
            await tc.ConfigureAwait(false);
        }
    }
}