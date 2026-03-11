using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminSecurityEvents;

public sealed class AdminSecurityEventsUseCase : UseCase<AdminSecurityEventsInput, IAdminSecurityEventsOutput>
{
    public override async Task ExecuteAsync(
        AdminSecurityEventsInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendQueryAsync(new AdminSecurityEventsQuery(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is not null)
        {
            Output.Ok(result.Payload<IReadOnlyCollection<AccountSecurityEventDto>>());
            return;
        }

        Output.Invalid("Account was not found or administrator access is required.");
    }
}
