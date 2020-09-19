namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Core.EventBus;
    using Aviant.DDD.Core.Events;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    public class AccountCreatedEventConsumer : EventHandler<AccountCreatedEvent>
    {
        private readonly UserManager<AccountUser> _userManager;

        public AccountCreatedEventConsumer(UserManager<AccountUser> userManager) => _userManager = userManager;

        public override async Task Handle(
            EventReceived<AccountCreatedEvent> @event,
            CancellationToken                  cancellationToken)
        {
            // It is the responsibility of previous steps validators to make sure that the creation of the user
            // will not fail (for example password property rules etc.)
            await _userManager.CreateAsync(
                    new AccountUser
                    {
                        Id        = @event.Event.Id.Key,
                        UserName  = @event.Event.UserName,
                        Email     = @event.Event.Email,
                        FirstName = @event.Event.FirstName,
                        LastName  = @event.Event.LastName
                    },
                    @event.Event.Password)
               .ConfigureAwait(false);
        }
    }
}