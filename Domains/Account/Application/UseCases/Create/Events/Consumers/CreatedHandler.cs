namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events.Consumers
{
    #region

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.EventBus;

    #endregion

    public class CreatedHandler : Aviant.DDD.Domain.Events.EventHandler<AccountCreatedEvent>
    {
        public override Task Handle(EventReceived<AccountCreatedEvent> @event, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}