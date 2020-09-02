namespace CleanDDDArchitecture.Application.Accounts.Commands.CreateAccount
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;

    public class CreateAccount : Command<AccountEntity, AccountId>
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

    public class CreateAccountHandler : CommandHandler<CreateAccount, AccountEntity, AccountId>
    {
        public override async Task<AccountEntity> Handle(CreateAccount command, CancellationToken cancellationToken)
        {
            var account = AccountEntity.Create(command.FirstName, command.LastName, command.Email);

            return account;
        }
    }
}