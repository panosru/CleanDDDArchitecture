using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;

public interface IProfileAccountOutput : IUseCaseOutput
{
    public void Ok(AccountUser accountUser);

    public void Invalid(string message);
}
