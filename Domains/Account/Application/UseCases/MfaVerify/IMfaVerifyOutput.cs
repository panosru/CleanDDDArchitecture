using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaVerify;

public interface IMfaVerifyOutput : IUseCaseOutput
{
    void Ok(MfaRecoveryCodesResult response);

    void Invalid(string message);
}
