using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalUnlink;

public sealed record ExternalUnlinkInput(string Provider) : UseCaseInput
{
    internal string Provider { get; } = Provider;

    public sealed class ExternalUnlinkInputValidator : CommandValidator<ExternalUnlinkInput>
    {
        public ExternalUnlinkInputValidator() =>
            RuleFor(input => input.Provider).NotEmpty();
    }
}
