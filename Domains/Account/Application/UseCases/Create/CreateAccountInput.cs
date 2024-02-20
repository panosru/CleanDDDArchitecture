using System.ComponentModel.DataAnnotations;
using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using Aviant.Core.Collections.Extensions;
using FluentValidation;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

public sealed record CreateAccountInput(
    string              Password,
    string              FirstName,
    string              LastName,
    string              Email,
    IEnumerable<string> Roles,
    bool                EmailConfirmed) : UseCaseInput
{
    internal string Password { get; } = Password;

    internal string FirstName { get; } = FirstName;

    internal string LastName { get; } = LastName;

    internal string Email { get; } = Email;

    internal IEnumerable<string> Roles { get; } = Roles;

    internal bool EmailConfirmed { get; } = EmailConfirmed;

    #region Nested type: CreateAccountInputValidator

    public sealed class CreateAccountInputValidator : CommandValidator<CreateAccountInput>
    {
        private readonly UserManager<AccountUser> _userManager;
        private readonly EmailAddressAttribute _emailAddressAttribute = new();

        public CreateAccountInputValidator(UserManager<AccountUser> userManager)
        {
            _userManager = userManager;

            RuleFor(v => v.Email)
                .CustomAsync(CheckEmailAsync);
            
            RuleFor(v => v.Password)
               .CustomAsync(CheckPasswordAsync);
        }

        private async Task<bool> CheckEmailAsync(
            string email,
            ValidationContext<CreateAccountInput> context,
            CancellationToken cancellationToken)
        {
            // Check if the email is valid
            if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            {
                context.AddFailure(_userManager.ErrorDescriber.InvalidEmail(email).Description);

                return false;
            }
            
            // Check if the email is unique
            if (await _userManager.FindByEmailAsync(email)
               .ConfigureAwait(false) is not null)
            {
                context.AddFailure(_userManager.ErrorDescriber.DuplicateEmail(email).Description);
                
                return false;
            }
            
            return true;
        }

        private async Task<bool> CheckPasswordAsync(
            string                                password,
            ValidationContext<CreateAccountInput> context,
            CancellationToken                     cancellationToken)
        {
            var isValid = true;

            foreach (IPasswordValidator<AccountUser> passwordValidator in _userManager.PasswordValidators)
            {
                var result = await passwordValidator.ValidateAsync(_userManager, new AccountUser(), password)
                   .ConfigureAwait(false);

                if (result.Succeeded)
                    continue;

                result.Errors.ForAll(
                    error => context.AddFailure(error.Description));

                isValid = false;
            }

            return isValid;
        }
    }

    #endregion
}
