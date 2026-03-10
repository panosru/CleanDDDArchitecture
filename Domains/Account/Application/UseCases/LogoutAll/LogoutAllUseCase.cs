using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.LogoutAll;

public sealed class LogoutAllUseCase : UseCase<ILogoutAllOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await Orchestrator.SendCommandAsync(new LogoutAllCommand(), cancellationToken)
            .ConfigureAwait(false);

        Output.Ok();
    }
}
