namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class TodoItemDeleteUseCase
        : UseCase<TodoItemDeleteInput, ITodoItemDeleteOutput, ITodoDbContextWrite>
    {
        public override async Task Execute(TodoItemDeleteInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new DeleteTodoItemCommand
                {
                    Id = input.Id
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}