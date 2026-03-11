using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;

public interface IListTrustedDevicesOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<AccountTrustedDeviceDto> trustedDevices);
}
