namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.Collections.Generic;
    using Aviant.DDD.Application.UseCases;

    public sealed class CreateAccountInput : UseCaseInput
    {
        public CreateAccountInput(
            string userName,
            string password,
            string firstName,
            string lastName,
            string email,
            IEnumerable<string> roles,
            bool emailConfirmed)
        {
            UserName       = userName;
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
}