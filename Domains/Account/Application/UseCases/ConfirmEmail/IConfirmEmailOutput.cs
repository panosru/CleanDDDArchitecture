namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

using Aviant.Application.UseCases;

public interface IConfirmEmailOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}