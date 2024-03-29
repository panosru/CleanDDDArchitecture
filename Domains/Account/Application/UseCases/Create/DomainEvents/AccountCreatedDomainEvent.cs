// ReSharper disable MemberCanBeInternal

using System.Text;
using Aviant.Application.ApplicationEvents;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Core.EventSourcing.DomainEvents;
using Aviant.Core.EventSourcing.EventBus;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Jobs;
using CleanDDDArchitecture.Domains.Shared.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events;

public sealed record AccountCreatedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
{
    // ReSharper disable once UnusedMember.Local
    #pragma warning disable 8618
    private AccountCreatedDomainEvent()
    { }
    #pragma warning restore 8618

    public AccountCreatedDomainEvent(AccountAggregate accountAggregate)
        : base(accountAggregate)
    {
        Id             = accountAggregate.Id;
        UserName       = accountAggregate.UserName;
        Password       = accountAggregate.Password;
        FirstName      = accountAggregate.FirstName;
        LastName       = accountAggregate.LastName;
        Email          = accountAggregate.Email;
        Roles          = accountAggregate.Roles;
        EmailConfirmed = accountAggregate.EmailConfirmed;
    }

    public AccountAggregateId Id { get; private set; }

    public string UserName { get; private set; }

    public string Password { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public IEnumerable<string> Roles { get; private set; }

    public bool EmailConfirmed { get; private set; }

    #region Nested type: AccountCreatedDomainEventConsumer

    internal sealed class AccountCreatedDomainEventConsumer : DomainEventHandler<AccountCreatedDomainEvent>
    {
        private readonly UserManager<AccountUser> _userManager;
        private readonly IJobRunner _jobRunner;

        public AccountCreatedDomainEventConsumer(
            UserManager<AccountUser> userManager,
            IJobRunner jobRunner)
        {
            _userManager = userManager;
            _jobRunner = jobRunner;
        }
            

        public override async Task Handle(
            EventReceived<AccountCreatedDomainEvent> @event,
            CancellationToken                        cancellationToken)
        {
            AccountUser user = new()
            {
                Id             = @event.Event.Id.Key,
                UserName       = @event.Event.UserName,
                Email          = @event.Event.Email,
                FirstName      = @event.Event.FirstName,
                LastName       = @event.Event.LastName,
                EmailConfirmed = @event.Event.EmailConfirmed
            };

            // It is the responsibility of previous steps validators to make sure that the creation of the user
            // will not fail (for example password property rules etc.)
            IdentityResult userResult = await _userManager.CreateAsync(
                    user,
                    @event.Event.Password)
               .ConfigureAwait(false);

            if (userResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, @event.Event.Roles)
                    .ConfigureAwait(false);

                // Send email confirmation link
                _jobRunner.Run<SendEmailConfirmJob, SendEmailConfirmJobOptions>(
                    options =>
                    {
                        options.Email = user.Email;
                    });
            }
            else
            {
                //TODO: raise failed event
                Console.WriteLine($"User was not created: {userResult.ToString()}");
            }
        }
    }

    #endregion
}
