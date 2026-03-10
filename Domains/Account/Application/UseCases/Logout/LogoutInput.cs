using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Logout;

public sealed record LogoutInput(string RefreshToken) : UseCaseInput
{
    internal string RefreshToken { get; } = RefreshToken;
}
