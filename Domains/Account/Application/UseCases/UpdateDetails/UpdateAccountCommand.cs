namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Commands;

    internal sealed class UpdateAccountCommand : Command<AccountAggregate, AccountAggregateId>
    {
        public UpdateAccountCommand(
            AccountAggregateId aggregateId,
            string             firstName,
            string             lastName,
            string             email)
        {
            AggregateId = aggregateId;
            FirstName   = firstName;
            LastName    = lastName;
            Email       = email;
        }

        public AccountAggregateId AggregateId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }

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

            if (account is null)
                throw new ArgumentOutOfRangeException(
                    nameof(UpdateAccountCommand.AggregateId),
                    "Invalid account aggregateId");

            account.ChangeDetails(command.FirstName, command.LastName, command.Email);

            return account;
        }
    }
}