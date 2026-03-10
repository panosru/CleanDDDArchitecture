using Aviant.Application.Identity;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSuspend;

public sealed class AdminSuspendUseCase : UseCase<AdminSuspendInput, IAdminSuspendOutput>
{
    public override async Task ExecuteAsync(
        AdminSuspendInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator
            .SendCommandAsync(new AdminSuspendCommand(input.AccountId, input.Reason), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to suspend the account.");
    }
}
