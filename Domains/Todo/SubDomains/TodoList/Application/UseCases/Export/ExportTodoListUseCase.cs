using System.Text;
using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

public sealed class ExportTodoListUseCase
    : UseCase<ExportTodoListInput, IExportTodoListOutput>
{
    public override async Task ExecuteAsync(
        ExportTodoListInput input,
        CancellationToken   cancellationToken = default)
    {
        OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
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
