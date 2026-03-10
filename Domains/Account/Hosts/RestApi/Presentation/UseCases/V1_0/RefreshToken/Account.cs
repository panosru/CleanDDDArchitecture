using System.Net;
using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.RefreshToken;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.RefreshToken;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountRefreshToken)]
public sealed class Account
    : ApiController<RefreshTokenUseCase, Account>,
      IRefreshTokenOutput
{
    public Account([FromServices] RefreshTokenUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IRefreshTokenOutput.Ok(AuthResult response) => ViewModel = Ok(response);

    void IRefreshTokenOutput.Unauthorized() => ViewModel = Unauthorized();

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        await UseCase.ExecuteAsync(new RefreshTokenInput(dto.RefreshToken))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
