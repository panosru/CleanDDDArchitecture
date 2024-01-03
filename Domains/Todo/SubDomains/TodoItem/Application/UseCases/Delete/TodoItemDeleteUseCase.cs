using Aviant.Application.Orchestration;
using Aviant.Application.Persistence.UseCases;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete;

public sealed class TodoItemDeleteUseCase
    : UseCase<TodoItemDeleteInput, ITodoItemDeleteOutput, ITodoDbContextWrite>
{
    public override async Task ExecuteAsync(
        TodoItemDeleteInput input,
        CancellationToken   cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new DeleteTodoItemCommand(input.Id),
                cancellationToken)
           .ConfigureAwait(false);

        if (!requestResult.Succeeded)
            Output.Invalid(requestResult.Messages.First());
    }
}
