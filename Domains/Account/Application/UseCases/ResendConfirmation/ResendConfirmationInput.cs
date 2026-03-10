using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation;

public sealed record ResendConfirmationInput(string Email) : UseCaseInput
{
    internal string Email { get; } = Email;

    public sealed class ResendConfirmationInputValidator : CommandValidator<ResendConfirmationInput>
    {
        private readonly EmailAddressAttribute _emailAddressAttribute = new();

        public ResendConfirmationInputValidator()
        {
            RuleFor(input => input.Email)
                .Custom(ValidateEmail);
        }

        private void ValidateEmail(
            string email,
            ValidationContext<ResendConfirmationInput> context)
        {
            if (_emailAddressAttribute.IsValid(email))
                return;

            context.AddFailure(nameof(ResendConfirmationInput.Email), $"Email '{email}' is invalid.");
        }
    }
}
