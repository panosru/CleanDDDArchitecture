using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.SecurityEvents;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.SecurityEvents;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountProfile)]
public sealed class Account : ApiController<SecurityEventsUseCase, Account>, ISecurityEventsOutput
{
    public Account([FromServices] SecurityEventsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void ISecurityEventsOutput.Ok(IReadOnlyCollection<AccountSecurityEventDto> events) =>
        ViewModel = Ok(new AccountSecurityEventsResponse(events));

    [HttpGet("security-events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountSecurityEventsResponse))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetOwnSecurityEvents()
    {
        await UseCase.ExecuteAsync().ConfigureAwait(false);

        return ViewModel;
    }
}
