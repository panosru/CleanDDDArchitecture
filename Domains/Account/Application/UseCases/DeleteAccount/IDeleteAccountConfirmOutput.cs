using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

public interface IDeleteAccountConfirmOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
