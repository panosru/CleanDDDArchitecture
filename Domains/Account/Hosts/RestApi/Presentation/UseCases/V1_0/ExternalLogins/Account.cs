using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalLogins;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalLogins;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountExternalIdentity)]
public sealed class Account
    : ApiController<ExternalLoginsUseCase, Account>,
      IExternalLoginsOutput
{
    public Account([FromServices] ExternalLoginsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IExternalLoginsOutput.Ok(IReadOnlyCollection<Core.Identity.Dto.ExternalLoginDto> logins) =>
        ViewModel = Ok(new ExternalLoginsResponse(logins));

    [HttpGet("external")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExternalLoginsResponse))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Logins()
    {
        await UseCase.ExecuteAsync().ConfigureAwait(false);

        return ViewModel;
    }
}
