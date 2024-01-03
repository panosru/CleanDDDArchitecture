using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public sealed record AuthenticateInput(string Username, string Password) : UseCaseInput
{
    internal string Username { get; } = Username;

    internal string Password { get; } = Password;
}
