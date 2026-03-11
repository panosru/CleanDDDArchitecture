using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

public sealed record DeleteAccountConfirmInput(string Email, string Token) : UseCaseInput;

public sealed class DeleteAccountConfirmInputValidator : CommandValidator<DeleteAccountConfirmInput>
{
    private readonly EmailAddressAttribute _emailAddressAttribute = new();

    public DeleteAccountConfirmInputValidator()
    {
        RuleFor(input => input.Token).NotEmpty();
        RuleFor(input => input.Email)
            .Custom(
                (email, context) =>
                {
                    if (!_emailAddressAttribute.IsValid(email))
                        context.AddFailure(nameof(DeleteAccountConfirmInput.Email), $"Email '{email}' is invalid.");
                });
    }
}
