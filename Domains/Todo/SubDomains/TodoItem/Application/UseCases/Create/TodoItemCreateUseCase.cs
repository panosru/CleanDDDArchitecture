using Aviant.Application.Orchestration;
using Aviant.Application.Persistence.UseCases;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;

public sealed class TodoItemCreateUseCase
    : UseCase<TodoItemCreateInput, ITodoItemCreateOutput, ITodoDbContextWrite>
{
    public override async Task ExecuteAsync(
        TodoItemCreateInput input,
        CancellationToken   cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new CreateTodoItemCommand(
                    input.ListId,
                    input.Title),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
