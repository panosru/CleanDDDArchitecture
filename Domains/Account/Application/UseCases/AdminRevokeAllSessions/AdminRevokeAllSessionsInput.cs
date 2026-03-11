using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminRevokeAllSessions;

public sealed record AdminRevokeAllSessionsInput(Guid AccountId) : UseCaseInput;
