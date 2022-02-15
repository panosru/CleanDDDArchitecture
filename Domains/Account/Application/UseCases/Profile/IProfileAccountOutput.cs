namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;

using Aviant.Application.UseCases;
using Identity;

public interface IProfileAccountOutput : IUseCaseOutput
{
    public void Ok(AccountUser accountUser);

    public void Invalid(string message);
}
