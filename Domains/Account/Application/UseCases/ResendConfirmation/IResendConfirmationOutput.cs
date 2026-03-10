using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation;

public interface IResendConfirmationOutput : IUseCaseOutput
{
    public void Accepted();
}
