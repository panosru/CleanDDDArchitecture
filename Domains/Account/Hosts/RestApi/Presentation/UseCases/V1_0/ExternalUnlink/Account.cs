using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalUnlink;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalUnlink;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountExternalIdentity)]
[EnableRateLimiting("authenticated-sensitive")]
public sealed class Account
    : ApiController<ExternalUnlinkUseCase, Account>,
      IExternalUnlinkOutput
{
    public Account([FromServices] ExternalUnlinkUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IExternalUnlinkOutput.Ok() => ViewModel = NoContent();

    void IExternalUnlinkOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpDelete("external/{provider}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Unlink([FromRoute] string provider)
    {
        await UseCase.ExecuteAsync(new ExternalUnlinkInput(provider)).ConfigureAwait(false);

        return ViewModel;
    }
}
