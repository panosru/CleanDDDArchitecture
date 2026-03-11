using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetClaims;

public sealed record AdminGetClaimsInput(Guid AccountId) : UseCaseInput;
