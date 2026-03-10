using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaSetup;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.MfaSetup;

[ApiVersion("1.0")]
[FeatureGate(Features.Account2fa)]
public sealed class Account : ApiController<MfaSetupUseCase, Account>, IMfaSetupOutput
{
    public Account([FromServices] MfaSetupUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IMfaSetupOutput.Ok(MfaSetupResult response) => ViewModel = Ok(response);

    void IMfaSetupOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("mfa/totp/setup")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MfaSetupResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Setup()
    {
        await UseCase.ExecuteAsync(new MfaSetupInput()).ConfigureAwait(false);

        return ViewModel;
    }
}
