namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class TodoItemCreateUseCase
        : UseCase<TodoItemCreateInput, ITodoItemCreateOutput, ITodoDbContextWrite>
    {
        public override async Task ExecuteAsync(
            TodoItemCreateInput input,
            CancellationToken   cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                new CreateTodoItemCommand
                {
                    ListId = input.ListId,

                    Title = input.Title
                },
                cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}