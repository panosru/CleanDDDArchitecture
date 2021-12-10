namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails;

using Aggregates;
using Aviant.DDD.Application.Orchestration;
using Aviant.DDD.Application.UseCases;

public sealed class UpdateDetailsUseCase
    : UseCase<UpdateDetailsInput, IUpdateDetailsOutput, AccountAggregate, AccountAggregateId>
{
    public override async Task ExecuteAsync(
        UpdateDetailsInput input,
        CancellationToken  cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new UpdateAccountCommand(
                    new AccountAggregateId(input.Id),
                    input.FirstName,
                    input.LastName,
                    input.Email),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload<AccountAggregate>());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
