using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Logout;

public sealed class LogoutUseCase : UseCase<LogoutInput, ILogoutOutput>
{
    public override async Task ExecuteAsync(
        LogoutInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new LogoutCommand(input.RefreshToken),
                cancellationToken)
            .ConfigureAwait(false);

        if (requestResult.Succeeded && requestResult.Payload() is bool revoked && revoked)
            Output.Ok();
        else
            Output.Unauthorized();
    }
}
