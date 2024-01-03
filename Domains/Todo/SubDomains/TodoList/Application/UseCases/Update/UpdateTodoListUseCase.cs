using Aviant.Application.Orchestration;
using Aviant.Application.Persistence.UseCases;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;

public sealed class UpdateTodoListUseCase
    : UseCase<UpdateTodoListInput, IUpdateTodoListOutput, ITodoDbContextWrite>
{
    public override async Task ExecuteAsync(
        UpdateTodoListInput input,
        CancellationToken   cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new UpdateTodoListCommand(input.Id, input.Title),
                cancellationToken)
           .ConfigureAwait(false);

        if (!requestResult.Succeeded)
            Output.Invalid(requestResult.Messages.First());
    }
}
