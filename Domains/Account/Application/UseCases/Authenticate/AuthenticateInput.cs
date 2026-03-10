using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public sealed record AuthenticateInput(
    string Username,
    string Password,
    string? TwoFactorCode = null,
    string? RecoveryCode = null) : UseCaseInput
{
    internal string Username { get; } = Username;

    internal string Password { get; } = Password;

    internal string? TwoFactorCode { get; } = TwoFactorCode;

    internal string? RecoveryCode { get; } = RecoveryCode;
}
