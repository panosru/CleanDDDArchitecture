using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetRoles;

public sealed record AdminGetRolesInput(Guid AccountId) : UseCaseInput;
