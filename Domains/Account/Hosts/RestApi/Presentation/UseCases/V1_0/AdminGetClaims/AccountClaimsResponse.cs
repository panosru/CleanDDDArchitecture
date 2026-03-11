using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminGetClaims;

internal sealed record AccountClaimsResponse(IReadOnlyCollection<AccountClaimDto> Claims);
