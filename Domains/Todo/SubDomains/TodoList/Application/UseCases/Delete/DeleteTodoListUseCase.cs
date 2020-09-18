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
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new DeleteTodoListCommand
                {
                    Id = Input.Id
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            Input = new DeleteTodoListInput(GetDataByType<int>(data));
        }
    }
}