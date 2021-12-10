namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;

using Aviant.DDD.Application.Orchestration;
using Aviant.DDD.Application.UseCases;

public sealed class GetAllUseCase : UseCase<IGetAllOutput>
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                new GetTodosQuery(),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload<TodosVm>());
        else
            Output.Invalid(requestResult.Messages.First());
    }
}
