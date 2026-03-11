using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalBegin;

public sealed record ExternalBeginInput(string Provider, string RedirectUri) : UseCaseInput
{
    internal string Provider { get; } = Provider;

    internal string RedirectUri { get; } = RedirectUri;

    public sealed class ExternalBeginInputValidator : CommandValidator<ExternalBeginInput>
    {
        public ExternalBeginInputValidator()
        {
            RuleFor(input => input.Provider).NotEmpty();
            RuleFor(input => input.RedirectUri)
                .NotEmpty()
                .Must(
                    value => Uri.TryCreate(value, UriKind.Absolute, out _))
                .WithMessage("RedirectUri must be an absolute URL.");
        }
    }
}
