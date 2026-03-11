using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSecurityEvents;

public interface IAdminSecurityEventsOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<AccountSecurityEventDto> events);

    void Invalid(string message);
}
