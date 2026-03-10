using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResetPassword;

public interface IResetPasswordOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(IEnumerable<string> errors);
}
