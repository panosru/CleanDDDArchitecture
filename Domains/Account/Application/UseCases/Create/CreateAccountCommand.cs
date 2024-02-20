using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Application.EventSourcing.Commands;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

public sealed record CreateAccountCommand(
    string              Password,
    string              FirstName,
    string              LastName,
    string              Email,
    IEnumerable<string> Roles,
    bool                EmailConfirmed) : Command<AccountAggregate, AccountAggregateId>
{
    private string Password { get; } = Password;

    private string FirstName { get; } = FirstName;

    private string LastName { get; } = LastName;

    private string Email { get; } = Email;

    private IEnumerable<string> Roles { get; } = Roles;

    private bool EmailConfirmed { get; } = EmailConfirmed;

    #region Nested type: CreateAccountHandler

    public sealed class CreateAccountHandler
        : CommandHandler<CreateAccountCommand, AccountAggregate, AccountAggregateId>
    {
        private readonly UserManager<AccountUser> _userManager;

        public CreateAccountHandler(UserManager<AccountUser> userManager) => 
            _userManager = userManager;

        public override Task<AccountAggregate> Handle(
            CreateAccountCommand command,
            CancellationToken cancellationToken)
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("Identity requires a user store with email support.");
            
            return Task.FromResult(
                AccountAggregate.Create(
                    command.Email, // Use the email as the username
                    command.Password,
                    command.FirstName,
                    command.LastName,
                    command.Email,
                    command.Roles,
                    command.EmailConfirmed));
        }
            
    }

    #endregion
}
