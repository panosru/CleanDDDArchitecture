using Aviant.Core.EventSourcing.Services;
using CleanDDDArchitecture.Domains.Account.Core.Aggregates;
using Ardalis.GuardClauses;
using Aviant.Application.EventSourcing.Commands;
using Aviant.Core.Messages;
using CleanDDDArchitecture.Domains.Account.Core.ValueObjects;

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
        private readonly IMessages _messages;

        public UpdateAccountHandler(IEventsService<AccountAggregate, AccountAggregateId> eventsService, IMessages messages)
            : base(eventsService) => _messages = messages;

        public override async Task<AccountAggregate> Handle(
            UpdateAccountCommand command,
            CancellationToken    cancellationToken)
        {
            var account = await EventsService
               .RehydrateAsync(command.AggregateId, cancellationToken)
               .ConfigureAwait(false);

            Guard.Against.Null(account, nameof(command.AggregateId));

            if (!string.Equals(account.Email, command.Email, StringComparison.OrdinalIgnoreCase))
            {
                _messages.AddMessage("Use the change-email flow to update the email address.");
                return null!;
            }

            account.Rename(PersonName.From(command.FirstName, command.LastName));

            return account;
        }
    }

    #endregion
}
