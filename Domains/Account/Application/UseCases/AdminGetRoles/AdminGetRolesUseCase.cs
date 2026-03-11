using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminGetRoles;

public sealed class AdminGetRolesUseCase : UseCase<AdminGetRolesInput, IAdminGetRolesOutput>
{
    public override async Task ExecuteAsync(
        AdminGetRolesInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendQueryAsync(new AdminGetRolesQuery(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is not null)
        {
            Output.Ok(result.Payload<IReadOnlyCollection<string>>());
            return;
        }

        Output.Invalid("Account was not found or administrator access is required.");
    }
}
