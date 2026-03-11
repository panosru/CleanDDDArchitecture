using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalBegin;

public interface IExternalBeginOutput : IUseCaseOutput
{
    void Ok(ExternalAuthenticationStartDto response);

    void Invalid(string message);
}
