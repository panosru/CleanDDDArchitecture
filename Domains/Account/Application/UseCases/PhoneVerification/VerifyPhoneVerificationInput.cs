using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

public sealed record VerifyPhoneVerificationInput(string PhoneNumber, string Code, bool IsChange) : UseCaseInput
{
    internal string PhoneNumber { get; } = PhoneNumber;

    internal string Code { get; } = Code;

    internal bool IsChange { get; } = IsChange;
}

public sealed class VerifyPhoneVerificationInputValidator : CommandValidator<VerifyPhoneVerificationInput>
{
    private readonly PhoneAttribute _phoneAttribute = new();

    public VerifyPhoneVerificationInputValidator()
    {
        RuleFor(input => input.Code).NotEmpty();
        RuleFor(input => input.PhoneNumber)
            .Custom(
                (phoneNumber, context) =>
                {
                    if (!_phoneAttribute.IsValid(phoneNumber))
                        context.AddFailure(nameof(VerifyPhoneVerificationInput.PhoneNumber), "Phone number is invalid.");
                });
    }
}
