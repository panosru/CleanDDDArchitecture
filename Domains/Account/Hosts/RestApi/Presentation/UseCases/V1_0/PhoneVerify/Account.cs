using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.PhoneVerify;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountPhoneVerification)]
public sealed class Account : ApiController<VerifyPhoneVerificationUseCase, Account>, IVerifyPhoneVerificationOutput
{
    public Account([FromServices] VerifyPhoneVerificationUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IVerifyPhoneVerificationOutput.Ok() => ViewModel = Ok();

    void IVerifyPhoneVerificationOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("phone/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Verify([FromBody] VerifyPhoneVerificationDto dto)
    {
        await UseCase.ExecuteAsync(new VerifyPhoneVerificationInput(dto.PhoneNumber, dto.Code, false)).ConfigureAwait(false);

        return ViewModel;
    }
}
