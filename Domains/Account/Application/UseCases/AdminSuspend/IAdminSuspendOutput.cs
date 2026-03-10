using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSuspend;

public interface IAdminSuspendOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}
