using Aviant.Application.Orchestration;
using Aviant.Application.Persistence.UseCases;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;

public sealed class TodoItemUpdatedetailsUseCase
    : UseCase<TodoItemUpdateDetailsInput, ITodoItemUpdateDetailsOutput, ITodoDbContextWrite>
{
    public override async Task ExecuteAsync(
        TodoItemUpdateDetailsInput input,
        CancellationToken          cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new UpdateTodoItemDetailCommand(
                    input.Id,
                    input.ListId,
                    input.Priority,
                    input.Note
                ),
                cancellationToken)
           .ConfigureAwait(false);

        if (!requestResult.Succeeded)
            Output.Invalid(requestResult.Messages.First());
    }
}
