namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;

using Aviant.Foundation.Application.UseCases;
using Identity;

public interface IProfileAccountOutput : IUseCaseOutput
{
    public void Ok(AccountUser accountUser);

    public void Invalid(string message);
}
