namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper.Internal;
    using FluentValidation;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    public sealed class CreateAccountInputValidator : AbstractValidator<CreateAccountInput>
    {
        private readonly UserManager<AccountUser> _userManager;

        public CreateAccountInputValidator(UserManager<AccountUser> userManager)
        {
            _userManager = userManager;

            RuleFor(v => v.Password)
               .CustomAsync(CheckPassword);
        }

        private async Task<bool> CheckPassword(
            string              password,
            ValidationContext<CreateAccountInput> context,
            CancellationToken   cancellationToken)
        {
            var isValid = true;

            foreach (IPasswordValidator<AccountUser> passwordValidator in _userManager.PasswordValidators)
            {
                var result = await passwordValidator.ValidateAsync(_userManager, new AccountUser(), password);

                if (result.Succeeded)
                    continue;

                result.Errors.ForAll(
                    error => context.AddFailure(error.Description));

                isValid = false;
            }

            return isValid;
        }
    }
}