using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.GetBy;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[ApiVersion("1.0")]
[FeatureGate(Features.AccountGetBy)]
public sealed class Account
    : ApiController<GetAccountUseCase, Account>,
      IGetAccountOutput
{
    /// <inheritdoc />
    public Account([FromServices] GetAccountUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IGetAccountOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void IGetAccountOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="accountUser"></param>
    void IGetAccountOutput.Ok(AccountUser accountUser) =>
        ViewModel = Ok(new AccountGetByResponse(accountUser));

    #endregion

    // void IUpdateDetailsOutput.Ok(AccountAggregate accountAggregate) =>
    //     ViewModel = Ok(new AccountGetByResponse(accountAggregate));

    /// <summary>
    ///     Get Account
    /// </summary>
    /// <response code="200">The account.</response>
    /// <response code="404">Not Found.</response>
    /// <param name="id"></param>
    /// <returns>Account data.</returns>
    [HttpGet("{id:guid}", Name = "GetAccount")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountGetByResponse))]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Find))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAccount([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new GetAccountInput(id))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
