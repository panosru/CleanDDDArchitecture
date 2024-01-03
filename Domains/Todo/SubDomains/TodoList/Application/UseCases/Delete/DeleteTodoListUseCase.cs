using Aviant.Application.Orchestration;
using Aviant.Application.Persistence.UseCases;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

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
