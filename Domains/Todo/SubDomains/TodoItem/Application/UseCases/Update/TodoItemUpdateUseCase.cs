namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

using Aviant.DDD.Application.Orchestration;
using Aviant.DDD.Application.UseCases;
using Todo.Application.Persistence;

public sealed class TodoItemUpdateUseCase
    : UseCase<TodoItemUpdateInput, ITodoItemUpdateOutput, ITodoDbContextWrite>
{
    public override async Task ExecuteAsync(
        TodoItemUpdateInput input,
        CancellationToken   cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new UpdateTodoItemCommand(
                    input.Id,
                    input.Title,
                    input.Done
                ),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
