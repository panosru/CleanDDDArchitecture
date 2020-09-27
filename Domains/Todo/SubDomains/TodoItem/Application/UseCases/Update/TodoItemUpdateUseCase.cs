namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class TodoItemUpdateUseCase
        : UseCase<TodoItemUpdateInput, ITodoItemUpdateOutput, ITodoDbContextWrite>
    {
        public override async Task ExecuteAsync(
            TodoItemUpdateInput input,
            CancellationToken   cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                new UpdateTodoItemCommand
                {
                    Id    = input.Id,
                    Title = input.Title,
                    Done  = input.Done
                },
                cancellationToken)
               .ConfigureAwait(false);

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}