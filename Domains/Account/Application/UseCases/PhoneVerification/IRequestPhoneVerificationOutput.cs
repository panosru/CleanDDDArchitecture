using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

public interface IRequestPhoneVerificationOutput : IUseCaseOutput
{
    void Accepted();

    void Invalid(string message);
}
