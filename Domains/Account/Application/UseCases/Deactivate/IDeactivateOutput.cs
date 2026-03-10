using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Deactivate;

public interface IDeactivateOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}
