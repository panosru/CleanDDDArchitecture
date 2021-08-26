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

        private string FirstName { get; }

        private string LastName { get; }

        private string Email { get; }

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

                if (account is null)
                    throw new ArgumentOutOfRangeException(
                        nameof(AggregateId),
                        "Invalid account aggregateId");

                account.ChangeDetails(command.FirstName, command.LastName, command.Email);

                return account;
            }
        }

        #endregion
    }
}