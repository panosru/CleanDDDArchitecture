using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaDisable;

public interface IMfaDisableOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
