using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateClaims;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminUpdateClaims;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminUpdateClaimsUseCase, Account>, IAdminUpdateClaimsOutput
{
    public Account([FromServices] AdminUpdateClaimsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminUpdateClaimsOutput.Ok() => ViewModel = Ok();

    void IAdminUpdateClaimsOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPut("admin/{id:guid}/claims")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateClaims([FromRoute] Guid id, [FromBody] AdminUpdateClaimsDto dto)
    {
        await UseCase.ExecuteAsync(new AdminUpdateClaimsInput(id, dto.Claims)).ConfigureAwait(false);

        return ViewModel;
    }
}
