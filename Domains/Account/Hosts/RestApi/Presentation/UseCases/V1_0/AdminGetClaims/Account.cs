using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetClaims;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminGetClaims;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminGetClaimsUseCase, Account>, IAdminGetClaimsOutput
{
    public Account([FromServices] AdminGetClaimsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminGetClaimsOutput.Ok(IReadOnlyCollection<Core.Identity.Dto.AccountClaimDto> claims) =>
        ViewModel = Ok(new AccountClaimsResponse(claims));

    void IAdminGetClaimsOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpGet("admin/{id:guid}/claims")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountClaimsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetClaims([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminGetClaimsInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
