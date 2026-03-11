using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailConfirm;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ChangeEmailConfirm;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountChangeEmail)]
[EnableRateLimiting("public-recovery")]
public sealed class Account
    : ApiController<ChangeEmailConfirmUseCase, Account>,
      IChangeEmailConfirmOutput
{
    public Account([FromServices] ChangeEmailConfirmUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IChangeEmailConfirmOutput.Ok() =>
        ViewModel = Ok();

    void IChangeEmailConfirmOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    [HttpGet("change-email/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ConfirmEmailChange([FromQuery] ChangeEmailConfirmDto dto)
    {
        await UseCase.ExecuteAsync(new ChangeEmailConfirmInput(dto.CurrentEmail, dto.NewEmail, dto.Token))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
