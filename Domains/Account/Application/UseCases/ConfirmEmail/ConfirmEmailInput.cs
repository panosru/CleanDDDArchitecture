namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail
{
    using Aviant.DDD.Application.UseCases;

    public class ConfirmEmailInput : IUseCaseInput
    {
        public ConfirmEmailInput(string token, string email)
        {
            Token = token;
            Email = email;
        }

        public string Token { get; }

        public string Email { get; }
    }
}