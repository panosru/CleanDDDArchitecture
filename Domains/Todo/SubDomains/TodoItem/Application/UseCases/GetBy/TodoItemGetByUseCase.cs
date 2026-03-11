using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Aviant.Application.Persistence.Orchestration;
using Aviant.Core.Services;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;

public sealed class TodoItemGetByUseCase
    : UseCase<TodoItemGetByInput, ITodoItemGetByOutput>
{
    private static IOrchestrator<ITodoDbContextWrite> TodoOrchestrator =>
        ServiceLocator.ServiceContainer.GetRequiredService<IOrchestrator<ITodoDbContextWrite>>(
            typeof(IOrchestrator<ITodoDbContextWrite>));

    public override async Task ExecuteAsync(
        TodoItemGetByInput input,
        CancellationToken  cancellationToken = default)
    {
        OrchestratorResponse requestResult = await TodoOrchestrator.SendQueryAsync(
                new GetTodoItemQuery(input.Id),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
