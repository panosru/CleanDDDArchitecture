namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public sealed class CreateAccountInput : UseCaseInput
    {
        public CreateAccountInput(
            string userName,
            string password,
            string firstName,
            string lastName,
            string email)
        {
            UserName  = userName;
            Password  = password;
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;
        }

        internal string UserName { get; }

        internal string Password { get; }

        internal string FirstName { get; }

        internal string LastName { get; }

        internal string Email { get; }
    }
}