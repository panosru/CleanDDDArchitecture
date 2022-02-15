namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

using Aviant.Application.UseCases;

public interface IAuthenticateOutput : IUseCaseOutput
{
    public void Ok(object? @object);

    public void Unauthorized();
}
