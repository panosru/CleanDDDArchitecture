using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ConfirmEmail;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountConfirmEmail)]
public sealed class Account
    : ApiController<ConfirmEmailUseCase, Account>,
      IConfirmEmailOutput
{
    /// <inheritdoc />
    public Account([FromServices] ConfirmEmailUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IConfirmEmailOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IConfirmEmailOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    void IConfirmEmailOutput.Ok() =>
        ViewModel = Ok();

    #endregion

    /// <summary>
    ///     Confirm user email with token taken from authentication endpoint
    /// </summary>
    /// <response code="200">Account confirmation.</response>
    /// <response code="202">Email confirmed successfully.</response>
    /// <response code="401">Invalid email token.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="404">Not Found.</response>
    /// <param name="dto"></param>
    /// <returns>Account confirmation message.</returns>
    [HttpGet("confirm/{Token}/{Email}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Patch))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ConfirmEmail([FromRoute] ConfirmEmailDto dto)
    {
        await UseCase.ExecuteAsync(new ConfirmEmailInput(dto.Token, dto.Email))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
