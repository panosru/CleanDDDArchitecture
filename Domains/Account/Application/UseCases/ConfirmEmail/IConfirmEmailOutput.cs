using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

public interface IConfirmEmailOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}
