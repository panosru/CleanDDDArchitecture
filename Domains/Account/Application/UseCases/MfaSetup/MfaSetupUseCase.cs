using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaSetup;

public sealed class MfaSetupUseCase : UseCase<MfaSetupInput, IMfaSetupOutput>
{
    public override async Task ExecuteAsync(
        MfaSetupInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator.SendCommandAsync(new MfaSetupCommand(), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is MfaSetupTicket ticket)
        {
            Output.Ok(
                new MfaSetupResult
                {
                    SharedKey = ticket.SharedKey,
                    AuthenticatorUri = ticket.AuthenticatorUri,
                    IsEnabled = ticket.IsEnabled
                });
            return;
        }

        Output.Invalid("Unable to start MFA setup.");
    }
}
