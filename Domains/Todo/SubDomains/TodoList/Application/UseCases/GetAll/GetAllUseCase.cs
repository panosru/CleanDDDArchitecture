using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Aviant.Application.Persistence.Orchestration;
using Aviant.Core.Services;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;

public sealed class GetAllUseCase : UseCase<IGetAllOutput>
{
    private IOrchestrator<ITodoDbContextWrite> TodoOrchestrator =>
        ServiceLocator.ServiceContainer.GetRequiredService<IOrchestrator<ITodoDbContextWrite>>(
            typeof(IOrchestrator<ITodoDbContextWrite>));

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await TodoOrchestrator.SendQueryAsync(
                new GetTodosQuery(),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload<TodosVm>());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
