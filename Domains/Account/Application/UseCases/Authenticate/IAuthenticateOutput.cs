using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public interface IAuthenticateOutput : IUseCaseOutput
{
    public void Ok(object? @object);

    public void Unauthorized();
}
