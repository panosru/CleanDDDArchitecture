namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Commands;

    public class UpdateAccount : Command<AccountEntity, AccountId>
    {
        public UpdateAccount(
            AccountId id,
            string    firstName,
            string    lastName,
            string    email)
        {
            Id        = id;
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;
        }

        public AccountId Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }

    public class UpdateAccountHandler : CommandHandler<UpdateAccount, AccountEntity, AccountId>
    {
        public override async Task<AccountEntity> Handle(UpdateAccount command, CancellationToken cancellationToken)
        {
            var account = await EventsService.RehydrateAsync(command.Id);

            if (account is null)
                throw new ArgumentOutOfRangeException(nameof(UpdateAccount.Id), "Invalid account id");

            account.ChangeDetails(command.FirstName, command.LastName, command.Email);

            return account;
        }
    }
}