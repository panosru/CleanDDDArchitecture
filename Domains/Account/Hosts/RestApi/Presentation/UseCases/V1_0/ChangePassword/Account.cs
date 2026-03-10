using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangePassword;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ChangePassword;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[FeatureGate(Features.AccountChangePassword)]
public sealed class Account
    : ApiController<ChangePasswordUseCase, Account>,
      IChangePasswordOutput
{
    public Account([FromServices] ChangePasswordUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IChangePasswordOutput.Ok() =>
        ViewModel = Ok();

    void IChangePasswordOutput.Invalid(IEnumerable<string> errors) =>
        ViewModel = BadRequest(new { errors });

    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await UseCase.ExecuteAsync(new ChangePasswordInput(dto.CurrentPassword, dto.NewPassword))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
