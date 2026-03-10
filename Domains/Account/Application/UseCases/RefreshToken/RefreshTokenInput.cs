using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RefreshToken;

public sealed record RefreshTokenInput(string RefreshToken) : UseCaseInput
{
    internal string RefreshToken { get; } = RefreshToken;
}
