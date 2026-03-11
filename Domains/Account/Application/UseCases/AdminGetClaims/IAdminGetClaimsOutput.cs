using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetClaims;

public interface IAdminGetClaimsOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<AccountClaimDto> claims);

    void Invalid(string message);
}
