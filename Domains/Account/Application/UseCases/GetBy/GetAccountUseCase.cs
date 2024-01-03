using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy;

public sealed class GetAccountUseCase
    : UseCase<GetAccountInput, IGetAccountOutput>
{
    public override async Task ExecuteAsync(
        GetAccountInput   input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                new GetAccountQuery(input.Id),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded
         && requestResult.Payload() is not null)
            Output.Ok(requestResult.Payload<AccountUser>());
        else
            Output.Invalid("Account not found.");
    }
}
