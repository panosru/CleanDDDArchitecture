using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.TrustedDevices;

public sealed class RevokeTrustedDeviceUseCase
    : UseCase<RevokeTrustedDeviceInput, IRevokeTrustedDeviceOutput>
{
    public override async Task ExecuteAsync(
        RevokeTrustedDeviceInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new RevokeTrustedDeviceCommand(input.TrustedDeviceId), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is bool revoked && revoked)
        {
            Output.Ok();
            return;
        }

        Output.Invalid("Trusted device not found.");
    }
}
