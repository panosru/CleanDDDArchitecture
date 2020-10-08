namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public sealed class UpdateTodoListUseCase
        : UseCase<UpdateTodoListInput, IUpdateTodoListOutput, ITodoDbContextWrite>
    {
        public override async Task ExecuteAsync(
            UpdateTodoListInput input,
            CancellationToken   cancellationToken = default)
        {
            OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                    new UpdateTodoListCommand(input.Id, input.Title),
                    cancellationToken)
               .ConfigureAwait(false);

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}