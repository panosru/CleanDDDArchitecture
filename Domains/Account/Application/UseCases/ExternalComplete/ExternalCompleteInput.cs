using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalComplete;

public sealed record ExternalCompleteInput(
    string Provider,
    string Code,
    string State,
    string RedirectUri) : UseCaseInput
{
    internal string Provider { get; } = Provider;

    internal string Code { get; } = Code;

    internal string State { get; } = State;

    internal string RedirectUri { get; } = RedirectUri;

    public sealed class ExternalCompleteInputValidator : CommandValidator<ExternalCompleteInput>
    {
        public ExternalCompleteInputValidator()
        {
            RuleFor(input => input.Provider).NotEmpty();
            RuleFor(input => input.Code).NotEmpty();
            RuleFor(input => input.State).NotEmpty();
            RuleFor(input => input.RedirectUri)
                .NotEmpty()
                .Must(value => Uri.TryCreate(value, UriKind.Absolute, out _))
                .WithMessage("RedirectUri must be an absolute URL.");
        }
    }
}
