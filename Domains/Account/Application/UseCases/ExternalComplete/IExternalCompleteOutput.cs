using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalComplete;

public interface IExternalCompleteOutput : IUseCaseOutput
{
    void Ok(ExternalAuthenticationResultDto response);

    void Invalid(string message);
}
