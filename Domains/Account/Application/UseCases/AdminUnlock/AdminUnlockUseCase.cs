using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnlock;

public sealed class AdminUnlockUseCase : UseCase<AdminUnlockInput, IAdminUnlockOutput>
{
    public override async Task ExecuteAsync(
        AdminUnlockInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator
            .SendCommandAsync(new AdminUnlockCommand(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to unlock the account.");
    }
}
