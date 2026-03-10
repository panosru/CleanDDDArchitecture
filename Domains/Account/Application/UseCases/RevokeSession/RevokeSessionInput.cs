using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RevokeSession;

public sealed record RevokeSessionInput(Guid SessionId) : UseCaseInput
{
    internal Guid SessionId { get; } = SessionId;
}
