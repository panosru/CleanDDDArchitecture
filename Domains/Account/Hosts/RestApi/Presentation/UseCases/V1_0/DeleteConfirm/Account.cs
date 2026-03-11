using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.DeleteConfirm;

[ApiVersion("1.0")]
[AllowAnonymous]
[FeatureGate(Features.AccountDelete)]
[EnableRateLimiting("public-recovery")]
public sealed class Account : ApiController<DeleteAccountConfirmUseCase, Account>, IDeleteAccountConfirmOutput
{
    public Account([FromServices] DeleteAccountConfirmUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IDeleteAccountConfirmOutput.Ok() => ViewModel = Ok();

    void IDeleteAccountConfirmOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpGet("confirm-delete/{Token}/{Email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ConfirmDelete([FromRoute] DeleteAccountConfirmDto dto)
    {
        await UseCase.ExecuteAsync(new DeleteAccountConfirmInput(dto.Email, dto.Token)).ConfigureAwait(false);

        return ViewModel;
    }
}
