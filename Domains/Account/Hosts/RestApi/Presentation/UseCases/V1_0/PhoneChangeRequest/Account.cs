using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.PhoneChangeRequest;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountPhoneVerification)]
public sealed class Account : ApiController<RequestPhoneVerificationUseCase, Account>, IRequestPhoneVerificationOutput
{
    public Account([FromServices] RequestPhoneVerificationUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IRequestPhoneVerificationOutput.Accepted() => ViewModel = Accepted();

    void IRequestPhoneVerificationOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("phone/change/request")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RequestChange([FromBody] RequestPhoneVerificationDto dto)
    {
        await UseCase.ExecuteAsync(new RequestPhoneVerificationInput(dto.PhoneNumber, true)).ConfigureAwait(false);

        return ViewModel;
    }
}
