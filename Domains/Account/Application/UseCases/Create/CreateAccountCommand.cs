namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Commands;

    public sealed class CreateAccountCommand : Command<AccountAggregate, AccountAggregateId>
    {
        public CreateAccountCommand(
            string username,
            string password,
            string firstName,
            string lastName,
            string email,
            IEnumerable<string> roles,
            bool emailConfirmed)
        {
            UserName       = username;
            Password       = password;
            FirstName      = firstName;
            LastName       = lastName;
            Email          = email;
            Roles          = roles;
            EmailConfirmed = emailConfirmed;
        }

        internal string UserName { get; }

        internal string Password { get; }

        internal string FirstName { get; }

        internal string LastName { get; }

        internal string Email { get; }

        internal IEnumerable<string> Roles { get; }

        internal bool EmailConfirmed { get; }
    }

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
}