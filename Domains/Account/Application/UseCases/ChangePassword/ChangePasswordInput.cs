using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangePassword;

public sealed record ChangePasswordInput(string CurrentPassword, string NewPassword) : UseCaseInput
{
    internal string CurrentPassword { get; } = CurrentPassword;

    internal string NewPassword { get; } = NewPassword;

    public sealed class ChangePasswordInputValidator : CommandValidator<ChangePasswordInput>
    {
        private readonly UserManager<AccountUser> _userManager;

        public ChangePasswordInputValidator(UserManager<AccountUser> userManager)
        {
            _userManager = userManager;

            RuleFor(input => input.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required.");

            RuleFor(input => input.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.");

            RuleFor(input => input)
                .Custom((input, context) =>
                {
                    if (!string.Equals(input.CurrentPassword, input.NewPassword, StringComparison.Ordinal))
                        return;

                    context.AddFailure(nameof(ChangePasswordInput.NewPassword), "New password must be different from the current password.");
                });

            RuleFor(input => input.NewPassword)
                .CustomAsync(CheckPasswordAsync);
        }

        private async Task CheckPasswordAsync(
            string newPassword,
            ValidationContext<ChangePasswordInput> context,
            CancellationToken cancellationToken)
        {
            foreach (var passwordValidator in _userManager.PasswordValidators)
            {
                var result = await passwordValidator.ValidateAsync(_userManager, new AccountUser(), newPassword)
                    .ConfigureAwait(false);

                if (result.Succeeded)
                    continue;

                foreach (var error in result.Errors)
                    context.AddFailure(nameof(ChangePasswordInput.NewPassword), error.Description);
            }
        }
    }
}
