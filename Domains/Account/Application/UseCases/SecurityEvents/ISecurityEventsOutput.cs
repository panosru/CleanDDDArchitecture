using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.SecurityEvents;

public interface ISecurityEventsOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<AccountSecurityEventDto> events);
}
