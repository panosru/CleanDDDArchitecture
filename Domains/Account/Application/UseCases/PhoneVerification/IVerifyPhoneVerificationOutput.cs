using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

public interface IVerifyPhoneVerificationOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
