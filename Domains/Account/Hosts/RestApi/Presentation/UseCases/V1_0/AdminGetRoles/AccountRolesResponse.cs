namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminGetRoles;

internal sealed record AccountRolesResponse(IReadOnlyCollection<string> Roles);
