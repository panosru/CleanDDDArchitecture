using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalLogins;

public interface IExternalLoginsOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<ExternalLoginDto> logins);
}
