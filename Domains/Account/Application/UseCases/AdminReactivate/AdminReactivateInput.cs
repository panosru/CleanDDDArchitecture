using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminReactivate;

public sealed record AdminReactivateInput(Guid AccountId) : UseCaseInput;
