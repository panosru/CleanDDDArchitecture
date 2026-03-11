using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetRoles;

public interface IAdminGetRolesOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<string> roles);

    void Invalid(string message);
}
