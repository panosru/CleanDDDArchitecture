namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class TodoItemUpdatedetailsUseCase
        : UseCase<TodoItemUpdateDetailsInput, ITodoItemUpdateDetailsOutput, ITodoDbContextWrite>
    {
        public override async Task Execute(TodoItemUpdateDetailsInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new UpdateTodoItemDetailCommand
                {
                    Id       = input.Id,
                    ListId   = input.ListId,
                    Note     = input.Note,
                    Priority = input.Priority
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }
    }
}