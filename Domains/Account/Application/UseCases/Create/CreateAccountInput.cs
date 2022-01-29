namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

using Aviant.DDD.Application.UseCases;
using Aviant.DDD.Core.Collections.Extensions;
using FluentValidation;
using Identity;
using Microsoft.AspNetCore.Identity;

public sealed record CreateAccountInput(
    string              UserName,
    string              Password,
    string              FirstName,
    string              LastName,
    string              Email,
    IEnumerable<string> Roles,
    bool                EmailConfirmed) : UseCaseInput
{
    internal string UserName { get; } = UserName;

    internal string Password { get; } = Password;

    internal string FirstName { get; } = FirstName;

    internal string LastName { get; } = LastName;

    internal string Email { get; } = Email;

    internal IEnumerable<string> Roles { get; } = Roles;

    internal bool EmailConfirmed { get; } = EmailConfirmed;

    #region Nested type: CreateAccountInputValidator

    public sealed class CreateAccountInputValidator : AbstractValidator<CreateAccountInput>
    {
        private readonly UserManager<AccountUser> _userManager;

        public CreateAccountInputValidator(UserManager<AccountUser> userManager)
        {
            _userManager = userManager;

            RuleFor(v => v.Password)
               .CustomAsync(CheckPasswordAsync);
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
