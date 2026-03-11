using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResetPassword;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ResetPassword;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountResetPassword)]
[EnableRateLimiting("public-recovery")]
public sealed class Account
    : ApiController<ResetPasswordUseCase, Account>,
      IResetPasswordOutput
{
    public Account([FromServices] ResetPasswordUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IResetPasswordOutput.Ok() =>
        ViewModel = Ok();

    void IResetPasswordOutput.Invalid(IEnumerable<string> errors) =>
        ViewModel = BadRequest(new { errors });

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await UseCase.ExecuteAsync(new ResetPasswordInput(dto.Email, dto.Token, dto.Password))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
