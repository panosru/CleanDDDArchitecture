using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaRecoveryCodes;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.MfaRecoveryCodes;

[ApiVersion("1.0")]
[FeatureGate(Features.Account2fa)]
public sealed class Account : ApiController<MfaRecoveryCodesUseCase, Account>, IMfaRecoveryCodesOutput
{
    public Account([FromServices] MfaRecoveryCodesUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IMfaRecoveryCodesOutput.Ok(MfaRecoveryCodesResult response) => ViewModel = Ok(response);

    void IMfaRecoveryCodesOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("mfa/recovery-codes/regenerate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MfaRecoveryCodesResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Regenerate([FromBody] MfaRecoveryCodesDto dto)
    {
        await UseCase.ExecuteAsync(new MfaRecoveryCodesInput(dto.CurrentPassword)).ConfigureAwait(false);

        return ViewModel;
    }
}
