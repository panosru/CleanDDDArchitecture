using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangePassword;

public interface IChangePasswordOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(IEnumerable<string> errors);
}
