namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events.Consumers
{
    #region

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Core.EventBus;

    #endregion

    public class UpdatedHandler : Aviant.DDD.Core.Events.EventHandler<AccountUpdatedEvent>
    {
        public override Task Handle(EventReceived<AccountUpdatedEvent> @event, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}