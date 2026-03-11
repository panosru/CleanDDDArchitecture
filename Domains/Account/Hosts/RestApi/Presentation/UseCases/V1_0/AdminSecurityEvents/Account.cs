using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSecurityEvents;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminSecurityEvents;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminSecurityEventsUseCase, Account>, IAdminSecurityEventsOutput
{
    public Account([FromServices] AdminSecurityEventsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminSecurityEventsOutput.Ok(IReadOnlyCollection<AccountSecurityEventDto> events) =>
        ViewModel = Ok(new SecurityEvents.AccountSecurityEventsResponse(events));

    void IAdminSecurityEventsOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpGet("admin/{id:guid}/security-events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SecurityEvents.AccountSecurityEventsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetSecurityEvents([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new AdminSecurityEventsInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
