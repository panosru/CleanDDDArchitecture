namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class TodoItemDeleteUseCase
        : UseCase<TodoItemDeleteInput, ITodoItemDeleteOutput, ITodoDbContextWrite>
    {
        public override async Task ExecuteAsync(
            TodoItemDeleteInput input,
            CancellationToken   cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                new DeleteTodoItemCommand
                {
                    Id = input.Id
                },
                cancellationToken)
               .ConfigureAwait(false);

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}