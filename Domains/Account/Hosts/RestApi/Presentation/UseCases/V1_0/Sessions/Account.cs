using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Sessions;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.Sessions;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[FeatureGate(Features.AccountSessions)]
public sealed class Account
    : ApiController<ListSessionsUseCase, Account>,
      IListSessionsOutput
{
    public Account([FromServices] ListSessionsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IListSessionsOutput.Ok(IReadOnlyCollection<AccountSessionDto> sessions) => ViewModel = Ok(sessions);

    [HttpGet("sessions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Sessions()
    {
        await UseCase.ExecuteAsync().ConfigureAwait(false);

        return ViewModel;
    }
}
