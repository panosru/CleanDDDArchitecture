namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Dtos;
    using Todo.Application.Persistence;

    public class CreateTodoListUseCase
        : UseCase<CreateTodoListInput, ICreateTodoListOutput, ITodoDbContextWrite>
    {
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new CreateTodoListCommand
                {
                    Title = Input.Title
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            var dto = GetDataByType<TodoListDto>(data);

            Input = new CreateTodoListInput(dto.Title);
        }
    }
}