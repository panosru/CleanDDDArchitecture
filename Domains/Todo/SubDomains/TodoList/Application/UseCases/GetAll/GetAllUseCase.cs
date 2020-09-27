namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class GetAllUseCase : UseCase<IGetAllOutput>
    {
        // private new IOrchestrator Orchestrator =>
        //     ServiceLocator.ServiceContainer.GetRequiredService<IOrchestrator>();

        public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendQueryAsync(
                new GetTodosQuery(),
                cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<TodosVm>());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}