using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalComplete;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalComplete;

[ApiVersion("1.0")]
[AllowAnonymous]
[FeatureGate(Features.AccountExternalIdentity)]
[EnableRateLimiting("public-auth")]
public sealed class Account
    : ApiController<ExternalCompleteUseCase, Account>,
      IExternalCompleteOutput
{
    public Account([FromServices] ExternalCompleteUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IExternalCompleteOutput.Ok(Core.Identity.Dto.ExternalAuthenticationResultDto response) => ViewModel = Ok(response);

    void IExternalCompleteOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("external/{provider}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Complete([FromRoute] string provider, [FromBody] ExternalCompleteDto dto)
    {
        await UseCase.ExecuteAsync(new ExternalCompleteInput(provider, dto.Code, dto.State, dto.RedirectUri)).ConfigureAwait(false);

        return ViewModel;
    }
}
