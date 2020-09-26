namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Dtos;
    using Todo.Application.Persistence;

    public class TodoItemUpdateUseCase
        : UseCase<TodoItemUpdateInput, ITodoItemUpdateOutput, ITodoDbContextWrite>
    {
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new UpdateTodoItemCommand
                {
                    Id    = Input.Id,
                    Title = Input.Title,
                    Done  = Input.Done
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }

        public TodoItemUpdateUseCase SetInput(TodoItemUpdateDto dto)
        {
            Input = new TodoItemUpdateInput(dto.Id, dto.Title, dto.Done);

            return this;
        }
    }
}