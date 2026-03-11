using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ResendConfirmation;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountResendConfirmation)]
[EnableRateLimiting("public-recovery")]
public sealed class Account
    : ApiController<ResendConfirmationUseCase, Account>,
      IResendConfirmationOutput
{
    public Account([FromServices] ResendConfirmationUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IResendConfirmationOutput.Accepted() =>
        ViewModel = Accepted();

    [HttpPost("resend-confirmation")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationDto dto)
    {
        await UseCase.ExecuteAsync(new ResendConfirmationInput(dto.Email))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
