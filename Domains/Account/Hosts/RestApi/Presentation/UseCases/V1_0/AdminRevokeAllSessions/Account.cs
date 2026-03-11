using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminRevokeAllSessions;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminRevokeAllSessions;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminRevokeAllSessionsUseCase, Account>, IAdminRevokeAllSessionsOutput
{
    public Account([FromServices] AdminRevokeAllSessionsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminRevokeAllSessionsOutput.Ok(int revokedSessions) =>
        ViewModel = Ok(new AdminRevokeAllSessionsResponse(revokedSessions));

    void IAdminRevokeAllSessionsOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("admin/{id:guid}/revoke-sessions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminRevokeAllSessionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RevokeAllSessions([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminRevokeAllSessionsInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
