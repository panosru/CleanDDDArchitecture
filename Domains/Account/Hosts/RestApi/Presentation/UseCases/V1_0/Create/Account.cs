using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Aviant.Core.Enum;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Shared.Core.Identity;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.Create;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[ApiVersion("1.0")]
[AllowAnonymous]
[FeatureGate(Features.AccountCreate)]
public sealed class Account
    : ApiController<AccountCreateUseCase, Account>,
      ICreateAccountOutput
{
    /// <inheritdoc />
    public Account([FromServices] AccountCreateUseCase useCase)
        : base(useCase) => useCase.SetOutput(this);

    #region ICreateAccountOutput Members

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    void ICreateAccountOutput.Invalid(string message) =>
        ViewModel = BadRequest(message);

    /// <summary>
    /// </summary>
    /// <param name="accountAggregate"></param>
    void ICreateAccountOutput.Ok(AccountAggregate accountAggregate) =>
        ViewModel = CreatedAtAction(
            "GetAccount",
            new
                { id = accountAggregate.Id },
            new AccountCreateResponse(accountAggregate));

    #endregion

    /// <summary>
    ///     Create a new account
    /// </summary>
    /// <response code="200">Account already exists</response>
    /// <response code="201">Account created successfully</response>
    /// <response code="400">Bad request.</response>
    /// <param name="dto"></param>
    /// <returns>The newly registered account.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK,      Type = typeof(AccountCreateResponse))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountCreateResponse))]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create([FromBody] [Required] CreateAccountDto dto)
    {
        await UseCase.ExecuteAsync(
                new CreateAccountInput(
                    dto.UserName,
                    dto.Password,
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    new[] { Roles.Member.ToString(StringCase.Lower) },
                    false))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
