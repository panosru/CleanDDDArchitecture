using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Aviant.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Deactivate;

public sealed class DeactivateUseCase : UseCase<DeactivateInput, IDeactivateOutput>
{
    public override async Task ExecuteAsync(
        DeactivateInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator
            .SendCommandAsync(new DeactivateCommand(input.CurrentPassword), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is IdentityResult { Succeeded: true })
        {
            Output.Ok();
            return;
        }

        Output.Invalid(result.Messages.FirstOrDefault() ?? "Unable to deactivate the account.");
    }
}
