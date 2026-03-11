using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;

public interface IRevokeTrustedDeviceOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
