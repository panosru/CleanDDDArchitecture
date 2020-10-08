namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public sealed class TodoItemUpdatedetailsUseCase
        : UseCase<TodoItemUpdateDetailsInput, ITodoItemUpdateDetailsOutput, ITodoDbContextWrite>
    {
        public override async Task ExecuteAsync(
            TodoItemUpdateDetailsInput input,
            CancellationToken          cancellationToken = default)
        {
            OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                    new UpdateTodoItemDetailCommand(
                        input.Id,
                        input.ListId,
                        input.Priority,
                        input.Note
                    ),
                    cancellationToken)
               .ConfigureAwait(false);

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}