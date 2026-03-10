using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaVerify;

public sealed class MfaVerifyUseCase : UseCase<MfaVerifyInput, IMfaVerifyOutput>
{
    public override async Task ExecuteAsync(
        MfaVerifyInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator.SendCommandAsync(
                new MfaVerifyCommand(input.Code),
                cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is MfaRecoveryCodesTicket ticket)
        {
            Output.Ok(new MfaRecoveryCodesResult { RecoveryCodes = ticket.RecoveryCodes });
            return;
        }

        Output.Invalid("Invalid two-factor authentication code.");
    }
}
