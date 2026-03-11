using System.Text;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Aviant.Application.Persistence.Orchestration;
using Aviant.Core.Services;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

public sealed class ExportTodoListUseCase
    : UseCase<ExportTodoListInput, IExportTodoListOutput>
{
    private IOrchestrator<ITodoDbContextWrite> TodoOrchestrator =>
        ServiceLocator.ServiceContainer.GetRequiredService<IOrchestrator<ITodoDbContextWrite>>(
            typeof(IOrchestrator<ITodoDbContextWrite>));

    public override async Task ExecuteAsync(
        ExportTodoListInput input,
        CancellationToken   cancellationToken = default)
    {
        OrchestratorResponse requestResult = await TodoOrchestrator.SendQueryAsync(
                new ExportTodosQuery(input.ListId),
                cancellationToken)
           .ConfigureAwait(false);

        if (requestResult.Succeeded)
            Output.Ok(requestResult.Payload<ExportTodosVm>());
        else
            Output.Invalid(
                Encoding.ASCII.GetBytes(
                    string.Join("\n\r", requestResult.Messages.ToArray())));
    }
}
