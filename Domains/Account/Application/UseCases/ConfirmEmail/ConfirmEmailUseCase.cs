using Aviant.Application.Orchestration;
using Aviant.Application.EventSourcing.UseCases;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;

public sealed class ConfirmEmailUseCase
    : UseCase<ConfirmEmailInput, IConfirmEmailOutput, AccountAggregate, AccountAggregateId>
{
    public override async Task ExecuteAsync(
        ConfirmEmailInput input,
        CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new ConfirmEmailCommand(
                    input.Token,
                    input.Email),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok();
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
