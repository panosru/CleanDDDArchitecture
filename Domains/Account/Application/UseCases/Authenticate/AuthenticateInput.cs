namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using Aviant.DDD.Application.UseCases;

    public class AuthenticateInput : UseCaseInput
    {
        public AuthenticateInput(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }

        public string Password { get; }
    }
}