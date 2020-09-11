namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Commands;

    public class CreateAccount : Command<AccountAggregate, AccountAggregateId>
    {
        public CreateAccount(
            string firstName,
            string lastName,
            string email)
        {
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }

    public class CreateAccountHandler : CommandHandler<CreateAccount, AccountAggregate, AccountAggregateId>
    {
        public override async Task<AccountAggregate> Handle(CreateAccount command, CancellationToken cancellationToken)
        {
            var account = AccountAggregate.Create(command.FirstName, command.LastName, command.Email);

            return account;
        }
    }
}