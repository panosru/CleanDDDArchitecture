namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail
{
    using Aviant.DDD.Application.UseCases;

    public sealed class ConfirmEmailInput : UseCaseInput
    {
        public ConfirmEmailInput(string token, string email)
        {
            Token = token;
            Email = email;
        }

        internal string Token { get; }

        internal string Email { get; }
    }
}