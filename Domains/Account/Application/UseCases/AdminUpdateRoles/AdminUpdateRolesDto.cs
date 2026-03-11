namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateRoles;

public sealed record AdminUpdateRolesDto(IReadOnlyCollection<string> Roles);
