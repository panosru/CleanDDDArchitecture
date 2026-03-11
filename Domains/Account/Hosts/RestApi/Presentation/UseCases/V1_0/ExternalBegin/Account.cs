using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalBegin;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalBegin;

[ApiVersion("1.0")]
[AllowAnonymous]
[FeatureGate(Features.AccountExternalIdentity)]
[EnableRateLimiting("public-auth")]
public sealed class Account
    : ApiController<ExternalBeginUseCase, Account>,
      IExternalBeginOutput
{
    public Account([FromServices] ExternalBeginUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IExternalBeginOutput.Ok(Core.Identity.Dto.ExternalAuthenticationStartDto response) => ViewModel = Ok(response);

    void IExternalBeginOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("external/{provider}/begin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Begin([FromRoute] string provider, [FromBody] ExternalBeginDto dto)
    {
        await UseCase.ExecuteAsync(new ExternalBeginInput(provider, dto.RedirectUri)).ConfigureAwait(false);

        return ViewModel;
    }
}
