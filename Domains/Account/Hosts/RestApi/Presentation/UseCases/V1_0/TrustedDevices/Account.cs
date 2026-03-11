using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.TrustedDevices;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountTrustedDevices)]
public sealed class Account : ApiController<ListTrustedDevicesUseCase, Account>, IListTrustedDevicesOutput
{
    public Account([FromServices] ListTrustedDevicesUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IListTrustedDevicesOutput.Ok(IReadOnlyCollection<AccountTrustedDeviceDto> trustedDevices) =>
        ViewModel = Ok(new TrustedDevicesResponse(trustedDevices));

    [HttpGet("trusted-devices")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrustedDevicesResponse))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> List()
    {
        await UseCase.ExecuteAsync().ConfigureAwait(false);

        return ViewModel;
    }
}
