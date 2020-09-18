namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class TodoItemGetByUseCase : UseCase<TodoItemGetByInput, ITodoItemGetByOutput>
    {
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendQuery(
                new GetTodoItemQuery
                {
                    Id = Input.Id
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            Input = new TodoItemGetByInput(GetDataByType<int>(data));
        }
    }
}