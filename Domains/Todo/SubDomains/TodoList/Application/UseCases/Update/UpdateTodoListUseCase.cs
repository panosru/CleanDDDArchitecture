namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class UpdateTodoListUseCase
        : UseCase<UpdateTodoListInput, IUpdateTodoListOutput, ITodoDbContextWrite>
    {
        public override async Task Execute(UpdateTodoListInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new UpdateTodoListCommand
                {
                    Id    = input.Id,
                    Title = input.Title
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}