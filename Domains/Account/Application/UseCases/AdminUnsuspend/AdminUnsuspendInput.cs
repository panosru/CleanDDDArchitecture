using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnsuspend;

public sealed record AdminUnsuspendInput(Guid AccountId) : UseCaseInput;
