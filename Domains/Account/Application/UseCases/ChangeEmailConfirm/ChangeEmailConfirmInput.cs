using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailConfirm;

public sealed record ChangeEmailConfirmInput(string CurrentEmail, string NewEmail, string Token) : UseCaseInput
{
    internal string CurrentEmail { get; } = CurrentEmail;

    internal string NewEmail { get; } = NewEmail;

    internal string Token { get; } = Token;
}
