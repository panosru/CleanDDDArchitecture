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
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new UpdateTodoItemDetailCommand
                {
                    Id       = Input.Id,
                    ListId   = Input.ListId,
                    Note     = Input.Note,
                    Priority = Input.Priority
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            var dto = GetDataByType<TodoItemUpdateDetailsDto>(data);

            Input = new TodoItemUpdateDetailsInput(
                dto.Id,
                dto.ListId,
                dto.Priority,
                dto.Note);
        }
    }
}