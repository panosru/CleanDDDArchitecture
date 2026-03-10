using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnlock;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminUnlock;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminUnlockUseCase, Account>, IAdminUnlockOutput
{
    public Account([FromServices] AdminUnlockUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminUnlockOutput.Ok() => ViewModel = Ok();

    void IAdminUnlockOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("admin/{id:guid}/unlock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Unlock([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminUnlockInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
