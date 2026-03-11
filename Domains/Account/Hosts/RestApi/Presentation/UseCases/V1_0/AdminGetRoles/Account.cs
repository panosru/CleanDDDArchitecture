using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetRoles;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminGetRoles;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminGetRolesUseCase, Account>, IAdminGetRolesOutput
{
    public Account([FromServices] AdminGetRolesUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminGetRolesOutput.Ok(IReadOnlyCollection<string> roles) => ViewModel = Ok(new AccountRolesResponse(roles));

    void IAdminGetRolesOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpGet("admin/{id:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountRolesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetRoles([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminGetRolesInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
