using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateRoles;

public sealed record AdminUpdateRolesInput(Guid AccountId, IReadOnlyCollection<string> Roles) : UseCaseInput;
