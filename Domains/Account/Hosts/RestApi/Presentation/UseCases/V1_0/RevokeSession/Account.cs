using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.RevokeSession;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.RevokeSession;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[FeatureGate(Features.AccountSessions)]
public sealed class Account
    : ApiController<RevokeSessionUseCase, Account>,
      IRevokeSessionOutput
{
    public Account([FromServices] RevokeSessionUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IRevokeSessionOutput.Ok() => ViewModel = NoContent();

    void IRevokeSessionOutput.Invalid(string message) => ViewModel = NotFound(message);

    [HttpDelete("sessions/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Delete))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RevokeSession([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new RevokeSessionInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
