namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using System.Text;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class ExportTodoListUseCase
        : UseCase<ExportTodoListInput, IExportTodoListOutput>
    {
        public override async Task Execute(ExportTodoListInput input)
        {
            RequestResult requestResult = await Orchestrator.SendQuery(
                new ExportTodosQuery
                {
                    ListId = input.ListId
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<ExportTodosVm>());
            else
                Output.Invalid(
                    Encoding.ASCII.GetBytes(
                        string.Join("\n\r", requestResult.Messages.ToArray())));
        }
    }
}