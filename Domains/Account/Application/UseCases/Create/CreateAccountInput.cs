namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper.Internal;
    using Aviant.DDD.Application.UseCases;
    using FluentValidation;
    using Identity;
    using Microsoft.AspNetCore.Identity;

    public sealed class CreateAccountInput : UseCaseInput
    {
        public CreateAccountInput(
            string              userName,
            string              password,
            string              firstName,
            string              lastName,
            string              email,
            IEnumerable<string> roles,
            bool                emailConfirmed)
        {
            UserName       = userName;
            Password       = password;
            FirstName      = firstName;
            LastName       = lastName;
            Email          = email;
            Roles          = roles;
            EmailConfirmed = emailConfirmed;
        }

        internal string UserName { get; }

        internal string Password { get; }

        internal string FirstName { get; }

        internal string LastName { get; }

        internal string Email { get; }

        internal IEnumerable<string> Roles { get; }

        internal bool EmailConfirmed { get; }

        #region Nested type: CreateAccountInputValidator

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
}
