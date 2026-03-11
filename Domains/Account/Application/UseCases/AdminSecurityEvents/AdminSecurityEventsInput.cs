using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSecurityEvents;

public sealed record AdminSecurityEventsInput(Guid AccountId) : UseCaseInput;
