using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

public sealed record ConfirmEmailInput(string Token, string Email) : UseCaseInput
{
    internal string Token { get; } = Token;

    internal string Email { get; } = Email;
}
