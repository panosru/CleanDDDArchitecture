using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateRoles;

public interface IAdminUpdateRolesOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
