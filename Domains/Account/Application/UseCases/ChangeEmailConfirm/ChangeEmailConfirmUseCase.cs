using Aviant.Application.EventSourcing.UseCases;
using Aviant.Application.Orchestration;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailConfirm;

public sealed class ChangeEmailConfirmUseCase
    : UseCase<ChangeEmailConfirmInput, IChangeEmailConfirmOutput, AccountAggregate, AccountAggregateId>
{
    public override async Task ExecuteAsync(
        ChangeEmailConfirmInput input,
        CancellationToken cancellationToken = default)
    {
        var requestResult = await Orchestrator.SendCommandAsync(
                new ChangeEmailConfirmCommand(input.CurrentEmail, input.NewEmail, input.Token),
                cancellationToken)
            .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok();
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
