namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Todo.Application.Persistence;

public sealed class DeleteTodoListUseCase
    : UseCase<DeleteTodoListInput, IDeleteTodoUseCaseOutput, ITodoDbContextWrite>
{
    public override async Task ExecuteAsync(
        DeleteTodoListInput input,
        CancellationToken   cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                new DeleteTodoListCommand(input.Id),
                cancellationToken)
           .ConfigureAwait(false);

        if (!requestResult.Succeeded)
            Output.Invalid(requestResult.Messages.First());
    }
}
