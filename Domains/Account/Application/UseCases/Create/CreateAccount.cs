namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    #region

    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Commands;

    #endregion

    public class CreateAccount : Command<AccountAggregate, AccountAggregateId>
    {
        public CreateAccount(
            string username,
            string password,
            string firstName,
            string lastName,
            string email)
        {
            UserName  = username;
            Password  = password;
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;
        }

        public string UserName { get; }

        public string Password { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }

    public class CreateAccountHandler : CommandHandler<CreateAccount, AccountAggregate, AccountAggregateId>
    {
        public override async Task<AccountAggregate> Handle(CreateAccount command, CancellationToken cancellationToken)
        {
            var account = AccountAggregate.Create(
                command.UserName,
                command.Password,
                command.FirstName,
                command.LastName,
                command.Email);

            return account;
        }
    }
}