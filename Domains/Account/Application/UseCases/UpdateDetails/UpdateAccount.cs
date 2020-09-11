namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    #region

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Commands;

    #endregion

    public class UpdateAccount : Command<AccountAggregate, AccountAggregateId>
    {
        public UpdateAccount(
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

    public class UpdateAccountHandler : CommandHandler<UpdateAccount, AccountAggregate, AccountAggregateId>
    {
        public override async Task<AccountAggregate> Handle(UpdateAccount command, CancellationToken cancellationToken)
        {
            var account = await EventsService.RehydrateAsync(command.AggregateId);

            if (account is null)
                throw new ArgumentOutOfRangeException(nameof(UpdateAccount.AggregateId), "Invalid account aggregateId");

            account.ChangeDetails(command.FirstName, command.LastName, command.Email);

            return account;
        }
    }
}