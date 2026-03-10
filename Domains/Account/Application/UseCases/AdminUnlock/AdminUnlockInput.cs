using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnlock;

public sealed record AdminUnlockInput(Guid AccountId) : UseCaseInput;
