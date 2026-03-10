using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.MfaDisable;

public sealed class MfaDisableUseCase : UseCase<MfaDisableInput, IMfaDisableOutput>
{
    public override async Task ExecuteAsync(
        MfaDisableInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse result = await Orchestrator.SendCommandAsync(
                new MfaDisableCommand(input.CurrentPassword),
                cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded)
            Output.Ok();
        else
            Output.Invalid(result.Messages.First());
    }
}
