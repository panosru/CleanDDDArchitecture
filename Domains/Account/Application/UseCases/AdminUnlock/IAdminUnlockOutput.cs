using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnlock;

public interface IAdminUnlockOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}
