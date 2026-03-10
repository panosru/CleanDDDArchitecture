using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword;

public sealed record ForgotPasswordInput(string Email) : UseCaseInput
{
    internal string Email { get; } = Email;

    public sealed class ForgotPasswordInputValidator : CommandValidator<ForgotPasswordInput>
    {
        private readonly EmailAddressAttribute _emailAddressAttribute = new();

        public ForgotPasswordInputValidator()
        {
            RuleFor(input => input.Email)
                .Custom(ValidateEmail);
        }

        private void ValidateEmail(
            string email,
            ValidationContext<ForgotPasswordInput> context)
        {
            if (_emailAddressAttribute.IsValid(email))
                return;

            context.AddFailure(nameof(ForgotPasswordInput.Email), $"Email '{email}' is invalid.");
        }
    }
}
