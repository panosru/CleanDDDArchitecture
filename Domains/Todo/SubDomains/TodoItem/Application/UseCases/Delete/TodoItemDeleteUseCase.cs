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
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new DeleteTodoItemCommand
                {
                    Id = Input.Id
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }

        public TodoItemDeleteUseCase SetInput(int id)
        {
            Input = new TodoItemDeleteInput(id);

            return this;
        }
    }
}