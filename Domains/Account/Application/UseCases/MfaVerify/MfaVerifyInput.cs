using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaVerify;

public sealed record MfaVerifyInput(string Code) : UseCaseInput
{
    internal string Code { get; } = Code;

    public sealed class MfaVerifyInputValidator : CommandValidator<MfaVerifyInput>
    {
        public MfaVerifyInputValidator()
        {
            RuleFor(input => input.Code)
                .NotEmpty()
                .WithMessage("Authenticator code is required.");
        }
    }
}
