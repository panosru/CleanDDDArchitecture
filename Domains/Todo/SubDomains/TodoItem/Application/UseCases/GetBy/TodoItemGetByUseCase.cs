namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public sealed class TodoItemGetByUseCase
        : UseCase<TodoItemGetByInput, ITodoItemGetByOutput>
    {
        public override async Task ExecuteAsync(
            TodoItemGetByInput input,
            CancellationToken  cancellationToken = default)
        {
            OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                    new GetTodoItemQuery(input.Id),
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}
