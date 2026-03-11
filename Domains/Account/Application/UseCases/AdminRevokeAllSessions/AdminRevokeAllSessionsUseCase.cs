using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminRevokeAllSessions;

public sealed class AdminRevokeAllSessionsUseCase : UseCase<AdminRevokeAllSessionsInput, IAdminRevokeAllSessionsOutput>
{
    public override async Task ExecuteAsync(
        AdminRevokeAllSessionsInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await Orchestrator
            .SendCommandAsync(new AdminRevokeAllSessionsCommand(input.AccountId), cancellationToken)
            .ConfigureAwait(false);

        if (result.Succeeded && result.Payload() is int revoked)
        {
            Output.Ok(revoked);
            return;
        }

        Output.Invalid("Account was not found or administrator access is required.");
    }
}
