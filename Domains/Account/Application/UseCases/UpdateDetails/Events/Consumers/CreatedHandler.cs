namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.EventBus;

    public class UpdatedHandler : Aviant.DDD.Domain.Events.EventHandler<AccountUpdatedEvent>
    {
        public override Task Handle(EventReceived<AccountUpdatedEvent> @event, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}