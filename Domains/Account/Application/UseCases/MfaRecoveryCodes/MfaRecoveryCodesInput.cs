using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaRecoveryCodes;

public sealed record MfaRecoveryCodesInput(string CurrentPassword) : UseCaseInput
{
    internal string CurrentPassword { get; } = CurrentPassword;

    public sealed class MfaRecoveryCodesInputValidator : CommandValidator<MfaRecoveryCodesInput>
    {
        public MfaRecoveryCodesInputValidator()
        {
            RuleFor(input => input.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required.");
        }
    }
}
