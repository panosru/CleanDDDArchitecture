using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateClaims;

public sealed record AdminUpdateClaimsInput(Guid AccountId, IReadOnlyCollection<AccountClaimDto> Claims) : UseCaseInput;
