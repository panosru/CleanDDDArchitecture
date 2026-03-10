using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ForgotPassword;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountForgotPassword)]
public sealed class Account
    : ApiController<ForgotPasswordUseCase, Account>,
      IForgotPasswordOutput
{
    public Account([FromServices] ForgotPasswordUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IForgotPasswordOutput.Accepted() =>
        ViewModel = Accepted();

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        await UseCase.ExecuteAsync(new ForgotPasswordInput(dto.Email))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
