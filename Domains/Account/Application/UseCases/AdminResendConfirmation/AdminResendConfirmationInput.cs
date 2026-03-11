using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminResendConfirmation;

public sealed record AdminResendConfirmationInput(Guid AccountId) : UseCaseInput;
