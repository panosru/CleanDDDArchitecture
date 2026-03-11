using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.RevokeTrustedDevice;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountTrustedDevices)]
public sealed class Account : ApiController<RevokeTrustedDeviceUseCase, Account>, IRevokeTrustedDeviceOutput
{
    public Account([FromServices] RevokeTrustedDeviceUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IRevokeTrustedDeviceOutput.Ok() => ViewModel = NoContent();

    void IRevokeTrustedDeviceOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpDelete("trusted-devices/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Revoke([FromRoute] Guid id)
    {
        await UseCase.ExecuteAsync(new RevokeTrustedDeviceInput(id)).ConfigureAwait(false);

        return ViewModel;
    }
}
