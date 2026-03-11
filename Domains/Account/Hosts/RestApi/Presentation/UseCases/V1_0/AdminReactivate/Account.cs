using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminReactivate;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminReactivate;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminReactivateUseCase, Account>, IAdminReactivateOutput
{
    public Account([FromServices] AdminReactivateUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminReactivateOutput.Ok() => ViewModel = Ok();

    void IAdminReactivateOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("admin/{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Reactivate([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminReactivateInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
