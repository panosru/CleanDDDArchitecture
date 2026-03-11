using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetClaims;

public sealed class AdminGetClaimsUseCase : UseCase<AdminGetClaimsInput, IAdminGetClaimsOutput>
{
    public override async Task ExecuteAsync(
        AdminGetClaimsInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendQueryAsync(new AdminGetClaimsQuery(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is not null)
        {
            Output.Ok(result.Payload<IReadOnlyCollection<AccountClaimDto>>());
            return;
        }

        Output.Invalid("Account was not found or administrator access is required.");
    }
}
