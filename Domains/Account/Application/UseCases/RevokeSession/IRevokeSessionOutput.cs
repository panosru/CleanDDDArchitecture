using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RevokeSession;

public interface IRevokeSessionOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}
