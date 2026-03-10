using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminUnsuspend;

public sealed class AdminUnsuspendUseCase : UseCase<AdminUnsuspendInput, IAdminUnsuspendOutput>
{
    public override async Task ExecuteAsync(
        AdminUnsuspendInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator
            .SendCommandAsync(new AdminUnsuspendCommand(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to unsuspend the account.");
    }
}
