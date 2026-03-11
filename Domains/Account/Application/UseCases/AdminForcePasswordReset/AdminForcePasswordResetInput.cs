using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminForcePasswordReset;

public sealed record AdminForcePasswordResetInput(Guid AccountId) : UseCaseInput;
