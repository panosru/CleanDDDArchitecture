using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public interface IAuthenticateOutput : IUseCaseOutput
{
    public void Ok(object? response);

    public void Unauthorized();
}
