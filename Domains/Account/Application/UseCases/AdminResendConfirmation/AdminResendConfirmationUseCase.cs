using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminResendConfirmation;

public sealed class AdminResendConfirmationUseCase : UseCase<AdminResendConfirmationInput, IAdminResendConfirmationOutput>
{
    public override async Task ExecuteAsync(
        AdminResendConfirmationInput input,
        CancellationToken cancellationToken = default)
    {
        await Orchestrator
            .SendCommandAsync(new AdminResendConfirmationCommand(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        Output.Accepted();
    }
}
