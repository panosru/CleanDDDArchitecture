namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Core.DomainEvents;
    using Aviant.DDD.Core.EventBus;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    internal sealed class AccountCreatedDomainEventConsumer : DomainEventHandler<AccountCreatedDomainEvent>
    {
        private readonly UserManager<AccountUser> _userManager;

        public AccountCreatedDomainEventConsumer(UserManager<AccountUser> userManager) => _userManager = userManager;

        public override async Task Handle(
            EventReceived<AccountCreatedDomainEvent> @event,
            CancellationToken                        cancellationToken)
        {
            AccountUser user = new()
            {
                Id        = @event.Event.Id.Key,
                UserName  = @event.Event.UserName,
                Email     = @event.Event.Email,
                FirstName = @event.Event.FirstName,
                LastName  = @event.Event.LastName,
                EmailConfirmed = @event.Event.EmailConfirmed
            };

            // It is the responsibility of previous steps validators to make sure that the creation of the user
            // will not fail (for example password property rules etc.)
            IdentityResult userResult = await _userManager.CreateAsync(
                    user,
                    @event.Event.Password)
               .ConfigureAwait(false);

            if (userResult.Succeeded)
                await _userManager.AddToRolesAsync(user, @event.Event.Roles);
            // else
            //TODO: raise failed event
        }
    }
}