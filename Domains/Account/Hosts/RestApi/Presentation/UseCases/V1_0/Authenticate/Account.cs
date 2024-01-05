using System.Net;
using System.Net.Mime;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.Authenticate;

/// <inheritdoc
///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.ApiController{TUseCase,TUseCaseOutput}" />
[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Features.AccountAuthentication)]
public sealed class Account
    : ApiController<AuthenticateUseCase, Account>,
      IAuthenticateOutput
{
    /// <inheritdoc />
    public Account([FromServices] AuthenticateUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    #region IAuthenticateOutput Members

    /// <summary>
    /// </summary>
    /// <param name="object"></param>
    void IAuthenticateOutput.Ok(object? @object) =>
        ViewModel = Ok(@object);

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    void IAuthenticateOutput.Unauthorized() =>
        throw new HttpRequestException(HttpStatusCode.Unauthorized.ToString());

    #endregion

    /// <summary>
    ///     Authenticate a user and a bearer or an email confirmation token
    /// </summary>
    /// <response code="200">Email confirmation required.</response>
    /// <response code="201">Login successful.</response>
    /// <response code="401">Invalid credentials or mail not confirmed.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="404">Not found.</response>
    /// <param name="dto"></param>
    /// <returns>Bearer token.</returns>
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateDto dto)
    {
        await UseCase.ExecuteAsync(new AuthenticateInput(dto.Username, dto.Password))
           .ConfigureAwait(false);

        return ViewModel;
    }
}
