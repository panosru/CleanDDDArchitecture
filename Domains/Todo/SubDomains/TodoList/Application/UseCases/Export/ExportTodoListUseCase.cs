namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class ExportTodoListUseCase
        : UseCase<ExportTodoListInput, IExportTodoListOutput>
    {
        public override async Task ExecuteAsync(
            ExportTodoListInput input,
            CancellationToken   cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendQueryAsync(
                new ExportTodosQuery
                {
                    ListId = input.ListId
                },
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
}