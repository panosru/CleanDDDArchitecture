using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateClaims;

public interface IAdminUpdateClaimsOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
