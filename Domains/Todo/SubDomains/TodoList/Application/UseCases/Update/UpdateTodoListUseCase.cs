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
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new UpdateTodoListCommand
                {
                    Id    = Input.Id,
                    Title = Input.Title
                });

            if (!requestResult.Succeeded)
                Output.Invalid(requestResult.Messages.First());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            var dto = GetDataByType<UpdateTodoListDto>(data);

            Input = new UpdateTodoListInput(dto.Id, dto.Title);
        }
    }
}