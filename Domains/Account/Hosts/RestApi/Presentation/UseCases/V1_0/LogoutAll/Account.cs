using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.LogoutAll;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.LogoutAll;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[FeatureGate(Features.AccountLogoutAll)]
public sealed class Account
    : ApiController<LogoutAllUseCase, Account>,
      ILogoutAllOutput
{
    public Account([FromServices] LogoutAllUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void ILogoutAllOutput.Ok() => ViewModel = NoContent();

    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> LogoutAll()
    {
        await UseCase.ExecuteAsync().ConfigureAwait(false);

        return ViewModel;
    }
}
