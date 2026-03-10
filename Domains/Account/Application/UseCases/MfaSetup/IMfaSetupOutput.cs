using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaSetup;

public interface IMfaSetupOutput : IUseCaseOutput
{
    void Ok(MfaSetupResult response);

    void Invalid(string message);
}
