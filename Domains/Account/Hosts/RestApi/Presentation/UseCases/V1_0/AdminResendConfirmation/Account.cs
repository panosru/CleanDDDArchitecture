using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminResendConfirmation;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminResendConfirmation;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminResendConfirmationUseCase, Account>, IAdminResendConfirmationOutput
{
    public Account([FromServices] AdminResendConfirmationUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminResendConfirmationOutput.Accepted() => ViewModel = Accepted();

    [HttpPost("admin/{id:guid}/resend-confirmation")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ResendConfirmation([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminResendConfirmationInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
