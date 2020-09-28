namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using Aviant.DDD.Application.UseCases;

    public sealed class AuthenticateInput : UseCaseInput
    {
        public AuthenticateInput(string username, string password)
        {
            Username = username;
            Password = password;
        }

        internal string Username { get; }

        internal string Password { get; }
    }
}