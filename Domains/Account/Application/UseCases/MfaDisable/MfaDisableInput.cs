using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaDisable;

public sealed record MfaDisableInput(string CurrentPassword) : UseCaseInput
{
    internal string CurrentPassword { get; } = CurrentPassword;

    public sealed class MfaDisableInputValidator : CommandValidator<MfaDisableInput>
    {
        public MfaDisableInputValidator()
        {
            RuleFor(input => input.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required.");
        }
    }
}
