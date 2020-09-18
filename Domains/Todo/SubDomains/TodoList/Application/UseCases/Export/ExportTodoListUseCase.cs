namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using System.Text;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using ViewModels;

    public class ExportTodoListUseCase : UseCase<ExportTodoListInput, IExportTodoListOutput>
    {
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendQuery(
                new ExportTodosQuery
                {
                    ListId = Input.ListId
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<ExportTodosVm>());
            else
                Output.Invalid(
                    Encoding.ASCII.GetBytes(
                        string.Join("\n\r", requestResult.Messages.ToArray())));
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            Input = new ExportTodoListInput(GetDataByType<int>(data));
        }
    }
}