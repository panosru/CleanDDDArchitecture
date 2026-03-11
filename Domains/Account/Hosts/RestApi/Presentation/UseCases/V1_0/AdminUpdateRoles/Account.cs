using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUpdateRoles;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminUpdateRoles;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminUpdateRolesUseCase, Account>, IAdminUpdateRolesOutput
{
    public Account([FromServices] AdminUpdateRolesUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminUpdateRolesOutput.Ok() => ViewModel = Ok();

    void IAdminUpdateRolesOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPut("admin/{id:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateRoles([FromRoute] Guid id, [FromBody] AdminUpdateRolesDto dto)
    {
        await UseCase.ExecuteAsync(new AdminUpdateRolesInput(id, dto.Roles)).ConfigureAwait(false);

        return ViewModel;
    }
}
