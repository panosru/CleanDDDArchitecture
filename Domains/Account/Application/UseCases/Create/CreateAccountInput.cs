namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public class CreateAccountInput : UseCaseInput
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

        public string UserName { get; }

        public string Password { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}