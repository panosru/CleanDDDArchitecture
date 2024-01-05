using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.Profile;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[ApiVersion("1.0")]
[FeatureGate(Features.AccountProfile)]
public sealed class Account
    : ApiController<ProfileAccountUseCase, Account>,
      IProfileAccountOutput
{
    /// <inheritdoc />
    public Account([FromServices] ProfileAccountUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IProfileAccountOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IProfileAccountOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="accountUser"></param>
    void IProfileAccountOutput.Ok(AccountUser accountUser) =>
        ViewModel = Ok(new AccountProfileResponse(accountUser));

    #endregion

    // void IUpdateDetailsOutput.Ok(AccountAggregate accountAggregate) =>
    //     ViewModel = Ok(new AccountGetByResponse(accountAggregate));

    /// <summary>
    ///     Get Current Account Profile
    /// </summary>
    /// <response code="200">The account.</response>
    /// <response code="404">Not Found.</response>
    /// <returns>Account Profile data.</returns>
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountProfileResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAccountProfile()
    {
        await UseCase.ExecuteAsync()
           .ConfigureAwait(false);

        return ViewModel;
    }
}
