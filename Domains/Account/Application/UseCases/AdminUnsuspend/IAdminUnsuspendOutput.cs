using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnsuspend;

public interface IAdminUnsuspendOutput : IUseCaseOutput
{
    public void Ok();

    public void Invalid(string message);
}
