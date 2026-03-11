using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

public sealed record DeleteAccountRequestInput(string CurrentPassword) : UseCaseInput;

public sealed class DeleteAccountRequestInputValidator : CommandValidator<DeleteAccountRequestInput>
{
    public DeleteAccountRequestInputValidator() =>
        RuleFor(input => input.CurrentPassword).NotEmpty();
}
