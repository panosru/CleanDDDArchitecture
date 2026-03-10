using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.LogoutAll;

public interface ILogoutAllOutput : IUseCaseOutput
{
    public void Ok();
}
