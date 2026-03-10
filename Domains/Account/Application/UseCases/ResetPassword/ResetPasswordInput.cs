using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResetPassword;

public sealed record ResetPasswordInput(string Email, string Token, string Password) : UseCaseInput
{
    internal string Email { get; } = Email;

    internal string Token { get; } = Token;

    internal string Password { get; } = Password;

    public sealed class ResetPasswordInputValidator : CommandValidator<ResetPasswordInput>
    {
        private readonly EmailAddressAttribute _emailAddressAttribute = new();
        private readonly UserManager<AccountUser> _userManager;

        public ResetPasswordInputValidator(UserManager<AccountUser> userManager)
        {
            _userManager = userManager;

            RuleFor(input => input.Email)
                .Custom(ValidateEmail);

            RuleFor(input => input.Token)
                .NotEmpty()
                .WithMessage("Password reset token is required.");

            RuleFor(input => input.Password)
                .CustomAsync(CheckPasswordAsync);
        }

        private void ValidateEmail(
            string email,
            ValidationContext<ResetPasswordInput> context)
        {
            if (_emailAddressAttribute.IsValid(email))
                return;

            context.AddFailure(nameof(ResetPasswordInput.Email), $"Email '{email}' is invalid.");
        }

        private async Task CheckPasswordAsync(
            string password,
            ValidationContext<ResetPasswordInput> context,
            CancellationToken cancellationToken)
        {
            foreach (var passwordValidator in _userManager.PasswordValidators)
            {
                var result = await passwordValidator.ValidateAsync(_userManager, new AccountUser(), password)
                    .ConfigureAwait(false);

                if (result.Succeeded)
                    continue;

                foreach (var error in result.Errors)
                    context.AddFailure(nameof(ResetPasswordInput.Password), error.Description);
            }
        }
    }
}
