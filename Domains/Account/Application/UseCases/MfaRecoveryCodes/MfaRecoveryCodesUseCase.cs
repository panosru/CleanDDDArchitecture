using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaRecoveryCodes;

public sealed class MfaRecoveryCodesUseCase : UseCase<MfaRecoveryCodesInput, IMfaRecoveryCodesOutput>
{
    public override async Task ExecuteAsync(
        MfaRecoveryCodesInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator.SendCommandAsync(
                new MfaRecoveryCodesCommand(input.CurrentPassword),
                cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is MfaRecoveryCodesTicket ticket)
        {
            Output.Ok(new MfaRecoveryCodesResult { RecoveryCodes = ticket.RecoveryCodes });
            return;
        }

        Output.Invalid("Unable to regenerate recovery codes.");
    }
}
