using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.Identity;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest;

public sealed record ChangeEmailRequestInput(string NewEmail) : UseCaseInput
{
    internal string NewEmail { get; } = NewEmail;

    public sealed class ChangeEmailRequestInputValidator : CommandValidator<ChangeEmailRequestInput>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly EmailAddressAttribute _emailAddressAttribute = new();
        private readonly UserManager<AccountUser> _userManager;

        public ChangeEmailRequestInputValidator(
            UserManager<AccountUser> userManager,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;

            RuleFor(input => input.NewEmail)
                .CustomAsync(ValidateEmailAsync);
        }

        private async Task ValidateEmailAsync(
            string newEmail,
            ValidationContext<ChangeEmailRequestInput> context,
            CancellationToken cancellationToken)
        {
            if (!_emailAddressAttribute.IsValid(newEmail))
            {
                context.AddFailure(nameof(ChangeEmailRequestInput.NewEmail), $"Email '{newEmail}' is invalid.");
                return;
            }

            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString()).ConfigureAwait(false);

            if (user is null)
            {
                context.AddFailure(nameof(ChangeEmailRequestInput.NewEmail), "User not found.");
                return;
            }

            if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                context.AddFailure(nameof(ChangeEmailRequestInput.NewEmail), "New email must be different from the current email.");
                return;
            }

            if (await _userManager.FindByEmailAsync(newEmail).ConfigureAwait(false) is not null)
                context.AddFailure(nameof(ChangeEmailRequestInput.NewEmail), _userManager.ErrorDescriber.DuplicateEmail(newEmail).Description);
        }
    }
}
