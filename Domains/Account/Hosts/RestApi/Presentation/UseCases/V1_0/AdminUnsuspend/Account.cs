using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnsuspend;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminUnsuspend;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminUnsuspendUseCase, Account>, IAdminUnsuspendOutput
{
    public Account([FromServices] AdminUnsuspendUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminUnsuspendOutput.Ok() => ViewModel = Ok();

    void IAdminUnsuspendOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("admin/{id:guid}/unsuspend")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Unsuspend([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminUnsuspendInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
