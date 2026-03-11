using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateClaims;

public sealed record AdminUpdateClaimsDto(IReadOnlyCollection<AccountClaimDto> Claims);
