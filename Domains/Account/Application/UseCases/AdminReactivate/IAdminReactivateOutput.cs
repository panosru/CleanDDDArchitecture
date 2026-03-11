using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminReactivate;

public interface IAdminReactivateOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
