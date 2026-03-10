using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailConfirm;

public interface IChangeEmailConfirmOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}
