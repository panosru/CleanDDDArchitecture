using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.TrustedDevices;

public sealed class TrustedDevicesResponse
{
    public TrustedDevicesResponse(IReadOnlyCollection<AccountTrustedDeviceDto> trustedDevices) => TrustedDevices = trustedDevices;

    public IReadOnlyCollection<AccountTrustedDeviceDto> TrustedDevices { get; }
}
