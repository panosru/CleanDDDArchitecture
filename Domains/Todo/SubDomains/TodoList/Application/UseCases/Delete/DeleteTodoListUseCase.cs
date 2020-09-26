namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class DeleteTodoListUseCase
        : UseCase<DeleteTodoListInput, IDeleteTodoUseCaseOutput, ITodoDbContextWrite>
    {
        public override async Task Execute(DeleteTodoListInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new DeleteTodoListCommand
                {
                    Id = input.Id
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}