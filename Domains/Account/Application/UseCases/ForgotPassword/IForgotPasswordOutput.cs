using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword;

public interface IForgotPasswordOutput : IUseCaseOutput
{
    public void Accepted();
}
