using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;

public sealed class ListTrustedDevicesUseCase : UseCase<IListTrustedDevicesOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendQueryAsync(new ListTrustedDevicesQuery(), cancellationToken)
            .ConfigureAwait(false);

        Output.Ok(
            result.Succeeded
                ? result.Payload<IReadOnlyCollection<AccountTrustedDeviceDto>>()
                : Array.Empty<AccountTrustedDeviceDto>());
    }
}
