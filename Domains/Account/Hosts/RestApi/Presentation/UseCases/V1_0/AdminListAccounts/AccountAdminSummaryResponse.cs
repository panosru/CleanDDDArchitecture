using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminListAccounts;

internal sealed record AccountAdminSummaryResponse(IReadOnlyCollection<AccountAdminSummaryDto> Accounts);
