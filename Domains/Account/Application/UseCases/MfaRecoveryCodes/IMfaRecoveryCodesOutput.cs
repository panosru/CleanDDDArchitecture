using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaRecoveryCodes;

public interface IMfaRecoveryCodesOutput : IUseCaseOutput
{
    void Ok(MfaRecoveryCodesResult response);

    void Invalid(string message);
}
