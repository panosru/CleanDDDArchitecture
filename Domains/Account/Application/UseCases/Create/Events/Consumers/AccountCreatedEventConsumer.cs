namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events.Consumers
{
    #region

    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.EventBus;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    #endregion

    public class AccountCreatedEventConsumer : Aviant.DDD.Domain.Events.EventHandler<AccountCreatedEvent>
    {
        private readonly UserManager<AccountUser> _userManager;

        public AccountCreatedEventConsumer(UserManager<AccountUser> userManager) => _userManager = userManager;

        public override async Task Handle(EventReceived<AccountCreatedEvent> @event, CancellationToken cancellationToken)
        {
            // It is the responsibility of previous steps validators to make sure that the creation of the user
            // will not fail (for example password property rules etc.)
            await _userManager.CreateAsync(
                new AccountUser
                {
                    UserName = @event.Event.UserName,
                    Email = @event.Event.Email,
                    FirstName = @event.Event.FirstName,
                    LastName = @event.Event.LastName
                }, @event.Event.Password)
               .ConfigureAwait(false);
        }
    }
}