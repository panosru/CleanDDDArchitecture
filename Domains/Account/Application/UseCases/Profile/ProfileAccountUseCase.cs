namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;

using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Identity;

public sealed class ProfileAccountUseCase
    : UseCase<IProfileAccountOutput>
{
    public override async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                new ProfileAccountQuery(),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded
            && requestResult.Payload() is not null)
            Output.Ok(requestResult.Payload<AccountUser>());
        else
            Output.Invalid("Account not found.");
    }
}
