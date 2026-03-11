using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminListAccounts;

public sealed class AdminListAccountsUseCase : UseCase<AdminListAccountsInput, IAdminListAccountsOutput>
{
    public override async Task ExecuteAsync(
        AdminListAccountsInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendQueryAsync(new AdminListAccountsQuery(input.Query, input.Status, input.Role), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded)
        {
            Output.Ok(result.Payload<IReadOnlyCollection<AccountAdminSummaryDto>>() ?? []);
            return;
        }

        Output.Invalid("Administrator access is required.");
    }
}
