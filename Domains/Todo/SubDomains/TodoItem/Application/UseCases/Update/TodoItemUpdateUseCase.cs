namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class TodoItemUpdateUseCase
        : UseCase<TodoItemUpdateInput, ITodoItemUpdateOutput, ITodoDbContextWrite>
    {
        public override async Task Execute(TodoItemUpdateInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new UpdateTodoItemCommand
                {
                    Id    = input.Id,
                    Title = input.Title,
                    Done  = input.Done
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}