using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Logout;

public interface ILogoutOutput : IUseCaseOutput
{
    public void Ok();

    public void Unauthorized();
}
