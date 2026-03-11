using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

public sealed record RequestPhoneVerificationInput(string PhoneNumber, bool IsChange) : UseCaseInput
{
    internal string PhoneNumber { get; } = PhoneNumber;

    internal bool IsChange { get; } = IsChange;
}

public sealed class RequestPhoneVerificationInputValidator : CommandValidator<RequestPhoneVerificationInput>
{
    private readonly PhoneAttribute _phoneAttribute = new();

    public RequestPhoneVerificationInputValidator() =>
        RuleFor(input => input.PhoneNumber)
            .Custom(
                (phoneNumber, context) =>
                {
                    if (!_phoneAttribute.IsValid(phoneNumber))
                        context.AddFailure(nameof(RequestPhoneVerificationInput.PhoneNumber), "Phone number is invalid.");
                });
}
