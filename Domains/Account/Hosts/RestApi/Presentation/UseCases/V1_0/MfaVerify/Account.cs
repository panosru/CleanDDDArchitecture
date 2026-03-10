using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaVerify;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.MfaVerify;

[ApiVersion("1.0")]
[FeatureGate(Features.Account2fa)]
public sealed class Account : ApiController<MfaVerifyUseCase, Account>, IMfaVerifyOutput
{
    public Account([FromServices] MfaVerifyUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IMfaVerifyOutput.Ok(MfaRecoveryCodesResult response) => ViewModel = Ok(response);

    void IMfaVerifyOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("mfa/totp/verify")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MfaRecoveryCodesResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Verify([FromBody] MfaVerifyDto dto)
    {
        await UseCase.ExecuteAsync(new MfaVerifyInput(dto.Code)).ConfigureAwait(false);

        return ViewModel;
    }
}
