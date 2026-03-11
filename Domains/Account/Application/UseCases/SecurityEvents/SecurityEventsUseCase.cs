using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.SecurityEvents;

public sealed class SecurityEventsUseCase : UseCase<ISecurityEventsOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendQueryAsync(new SecurityEventsQuery(), cancellationToken)
            .ConfigureAwait(false);

        Output.Ok(result.Payload<IReadOnlyCollection<AccountSecurityEventDto>>() ?? []);
    }
}
