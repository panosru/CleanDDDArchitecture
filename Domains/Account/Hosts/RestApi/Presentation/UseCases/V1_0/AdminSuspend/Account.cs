using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSuspend;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminSuspend;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminSuspendUseCase, Account>, IAdminSuspendOutput
{
    public Account([FromServices] AdminSuspendUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminSuspendOutput.Ok() => ViewModel = Ok();

    void IAdminSuspendOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("admin/{id:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Suspend([FromRoute] Guid id, [FromBody] AdminSuspendDto dto)
    {
        await UseCase.ExecuteAsync(new AdminSuspendInput(id, dto.Reason)).ConfigureAwait(false);

        return ViewModel;
    }
}
