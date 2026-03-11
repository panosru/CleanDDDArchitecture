using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;

public sealed record RevokeTrustedDeviceInput(Guid TrustedDeviceId) : UseCaseInput
{
    internal Guid TrustedDeviceId { get; } = TrustedDeviceId;
}
