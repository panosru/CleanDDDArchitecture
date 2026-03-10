using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Sessions;

public sealed class ListSessionsUseCase : UseCase<IListSessionsOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                new ListSessionsQuery(),
                cancellationToken)
            .ConfigureAwait(false);

        Output.Ok(
            requestResult.Succeeded
                ? requestResult.Payload<IReadOnlyCollection<AccountSessionDto>>()
                : Array.Empty<AccountSessionDto>());
    }
}
