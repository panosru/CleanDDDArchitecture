using Aviant.Application.Orchestration;
using Aviant.Application.Persistence.UseCases;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

/// <inheritdoc />
/// <summary>
///     Create todo list use case
/// </summary>
public sealed class CreateTodoListUseCase
    : UseCase<CreateTodoListInput, ICreateTodoListOutput, ITodoDbContextWrite>
{
    /// <inheritdoc />
    public override async Task ExecuteAsync(
        CreateTodoListInput input,
        CancellationToken   cancellationToken = default)
    {
        await ValidateInputAsync(input, cancellationToken)
           .ConfigureAwait(false);

        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new CreateTodoListCommand(input.Title),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
