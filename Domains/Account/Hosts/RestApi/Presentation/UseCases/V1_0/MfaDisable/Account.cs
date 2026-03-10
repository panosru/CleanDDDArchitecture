using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaDisable;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.MfaDisable;

[ApiVersion("1.0")]
[FeatureGate(Features.Account2fa)]
public sealed class Account : ApiController<MfaDisableUseCase, Account>, IMfaDisableOutput
{
    public Account([FromServices] MfaDisableUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IMfaDisableOutput.Ok() => ViewModel = Ok();

    void IMfaDisableOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("mfa/totp/disable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Disable([FromBody] MfaDisableDto dto)
    {
        await UseCase.ExecuteAsync(new MfaDisableInput(dto.CurrentPassword)).ConfigureAwait(false);

        return ViewModel;
    }
}
