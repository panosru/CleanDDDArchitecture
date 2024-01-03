using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Ardalis.GuardClauses;
using Aviant.Application.EventSourcing.Commands;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails;

internal sealed record UpdateAccountCommand(
    AccountAggregateId AggregateId,
    string             FirstName,
    string             LastName,
    string             Email) : Command<AccountAggregate, AccountAggregateId>
{
    private string FirstName { get; } = FirstName;

    private string LastName { get; } = LastName;

    private string Email { get; } = Email;

    #region Nested type: UpdateAccountHandler

    internal sealed class UpdateAccountHandler
        : CommandHandler<UpdateAccountCommand, AccountAggregate, AccountAggregateId>
    {
        public override async Task<AccountAggregate> Handle(
            UpdateAccountCommand command,
            CancellationToken    cancellationToken)
        {
            var account = await EventsService
               .RehydrateAsync(command.AggregateId, cancellationToken)
               .ConfigureAwait(false);

            Guard.Against.Null(account, nameof(command.AggregateId));

            account.ChangeDetails(command.FirstName, command.LastName, command.Email);

            return account;
        }
    }

    #endregion
}
