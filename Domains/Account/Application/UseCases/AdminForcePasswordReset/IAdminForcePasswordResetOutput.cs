using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminForcePasswordReset;

public interface IAdminForcePasswordResetOutput : IUseCaseOutput
{
    void Accepted();
}
