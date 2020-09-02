namespace CleanDDDArchitecture.Application.Accounts.Commands.CreateAccount
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.EventBus;
    using Aviant.DDD.Domain.Events;

    public class CreatedHandler : EventHandler<AccountCreatedEvent>
    {
        public override Task Handle(EventReceived<AccountCreatedEvent> @event, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}