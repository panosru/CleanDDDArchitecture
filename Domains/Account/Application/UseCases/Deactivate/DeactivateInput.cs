using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Deactivate;

public sealed record DeactivateInput(string CurrentPassword) : UseCaseInput;

public sealed class DeactivateInputValidator : CommandValidator<DeactivateInput>
{
    public DeactivateInputValidator() =>
        RuleFor(input => input.CurrentPassword)
            .NotEmpty();
}
