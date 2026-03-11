using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminForcePasswordReset;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminForcePasswordReset;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Admin : ApiController<AdminForcePasswordResetUseCase, Admin>, IAdminForcePasswordResetOutput
{
    public Admin([FromServices] AdminForcePasswordResetUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminForcePasswordResetOutput.Accepted() => ViewModel = Accepted();

    [HttpPost("admin/{id:guid}/force-password-reset")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ForcePasswordReset([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminForcePasswordResetInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
