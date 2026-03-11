using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminResendConfirmation;

public interface IAdminResendConfirmationOutput : IUseCaseOutput
{
    void Accepted();
}
