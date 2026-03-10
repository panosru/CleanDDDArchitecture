using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSuspend;

public sealed record AdminSuspendInput(Guid AccountId, string? Reason) : UseCaseInput;
