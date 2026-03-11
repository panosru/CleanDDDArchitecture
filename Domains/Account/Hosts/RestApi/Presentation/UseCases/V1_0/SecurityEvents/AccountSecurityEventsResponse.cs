using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.SecurityEvents;

internal sealed record AccountSecurityEventsResponse(IReadOnlyCollection<AccountSecurityEventDto> Events);
