using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Application.Orchestration;
using Aviant.Application.EventSourcing.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

public sealed class AccountCreateUseCase
    : UseCase<CreateAccountInput, ICreateAccountOutput, AccountAggregate, AccountAggregateId>
{
    public override async Task ExecuteAsync(
        CreateAccountInput input,
        CancellationToken  cancellationToken = default)
    {
        await ValidateInputAsync(input, cancellationToken)
           .ConfigureAwait(false);

        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new CreateAccountCommand(
                    input.UserName,
                    input.Password,
                    input.FirstName,
                    input.LastName,
                    input.Email,
                    input.Roles,
                    input.EmailConfirmed),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload<AccountAggregate>());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
