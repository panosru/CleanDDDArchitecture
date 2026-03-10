using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RevokeSession;

public sealed class RevokeSessionUseCase : UseCase<RevokeSessionInput, IRevokeSessionOutput>
{
    public override async Task ExecuteAsync(
        RevokeSessionInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new RevokeSessionCommand(input.SessionId),
                cancellationToken)
            .ConfigureAwait(false);

        if (requestResult.Succeeded && requestResult.Payload() is bool revoked && revoked)
            Output.Ok();
        else
            Output.Invalid("Session not found.");
    }
}
