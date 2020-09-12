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