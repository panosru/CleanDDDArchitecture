using System.Net;
using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Logout;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.Logout;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountLogout)]
public sealed class Account
    : ApiController<LogoutUseCase, Account>,
      ILogoutOutput
{
    public Account([FromServices] LogoutUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void ILogoutOutput.Ok() => ViewModel = NoContent();

    void ILogoutOutput.Unauthorized() => ViewModel = Unauthorized();

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
    {
        await UseCase.ExecuteAsync(new LogoutInput(dto.RefreshToken))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
