namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Dtos;
    using Todo.Application.Persistence;

    public class TodoItemCreateUseCase
        : UseCase<TodoItemCreateInput, ITodoItemCreateOutput, ITodoDbContextWrite>
    {
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new CreateTodoItemCommand
                {
                    ListId = Input.ListId,

                    Title = Input.Title
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }

        public TodoItemCreateUseCase SetInput(TodoItemCreateDto dto)
        {
            Input = new TodoItemCreateInput(dto.ListId, dto.Title);

            return this;
        }
    }
}