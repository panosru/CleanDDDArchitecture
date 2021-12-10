namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;

using Aviant.DDD.Application.Orchestration;
using Aviant.DDD.Application.UseCases;
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

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload<AccountUser>());
    }
}
