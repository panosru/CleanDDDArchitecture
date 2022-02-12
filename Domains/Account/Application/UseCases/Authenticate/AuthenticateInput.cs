namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

using Aviant.Foundation.Application.UseCases;

public sealed record AuthenticateInput(string Username, string Password) : UseCaseInput
{
    internal string Username { get; } = Username;

    internal string Password { get; } = Password;
}
