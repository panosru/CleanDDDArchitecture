namespace CleanDDDArchitecture.RestApi.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Accounts;
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Domain.EventBus;
    using Domain.Entities;
    using Infrastructure;
    using Microsoft.Extensions.Hosting;

    public class EventsConsumerWorker : BackgroundService
    {
        private readonly IEventConsumerFactory _eventConsumerFactory;

        public EventsConsumerWorker(IEventConsumerFactory eventConsumerFactory)
        {
            _eventConsumerFactory = eventConsumerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IEnumerable<IEventConsumer> consumers = new[]
            {
                _eventConsumerFactory.Build<AccountEntity, AccountId, AccountIdDeserializer>()
            };

            var tc = Task.WhenAll(consumers.Select(c => c.ConsumeAsync(stoppingToken)));
            await tc;
        }
    }
}