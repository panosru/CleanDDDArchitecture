using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

public interface IDeleteAccountRequestOutput : IUseCaseOutput
{
    void Accepted();

    void Invalid(string message);
}
