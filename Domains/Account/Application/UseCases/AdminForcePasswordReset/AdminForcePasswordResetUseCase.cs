using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminForcePasswordReset;

public sealed class AdminForcePasswordResetUseCase : UseCase<AdminForcePasswordResetInput, IAdminForcePasswordResetOutput>
{
    public override async Task ExecuteAsync(
        AdminForcePasswordResetInput input,
        CancellationToken cancellationToken = default)
    {
        await Orchestrator
            .SendCommandAsync(new AdminForcePasswordResetCommand(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        Output.Accepted();
    }
}
