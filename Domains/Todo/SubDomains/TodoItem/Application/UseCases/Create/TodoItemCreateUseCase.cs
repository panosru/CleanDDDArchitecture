namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public sealed class TodoItemCreateUseCase
        : UseCase<TodoItemCreateInput, ITodoItemCreateOutput, ITodoDbContextWrite>
    {
        public override async Task ExecuteAsync(
            TodoItemCreateInput input,
            CancellationToken   cancellationToken = default)
        {
            OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                    new CreateTodoItemCommand(
                        input.ListId,
                        input.Title),
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}