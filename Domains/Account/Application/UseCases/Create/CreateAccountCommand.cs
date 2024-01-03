using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Application.EventSourcing.Commands;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

public sealed record CreateAccountCommand(
    string              UserName,
    string              Password,
    string              FirstName,
    string              LastName,
    string              Email,
    IEnumerable<string> Roles,
    bool                EmailConfirmed) : Command<AccountAggregate, AccountAggregateId>
{
    private string UserName { get; } = UserName;

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
        public override Task<AccountAggregate> Handle(
            CreateAccountCommand command,
            CancellationToken    cancellationToken) =>
            Task.FromResult(
                AccountAggregate.Create(
                    command.UserName,
                    command.Password,
                    command.FirstName,
                    command.LastName,
                    command.Email,
                    command.Roles,
                    command.EmailConfirmed));
    }

    #endregion
}
