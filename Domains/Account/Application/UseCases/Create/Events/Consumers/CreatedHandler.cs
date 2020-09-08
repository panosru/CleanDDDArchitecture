namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.EventBus;

    public class CreatedHandler : Aviant.DDD.Domain.Events.EventHandler<AccountCreatedEvent>
    {
        public override Task Handle(EventReceived<AccountCreatedEvent> @event, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}