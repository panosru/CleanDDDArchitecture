namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public class CreateTodoListUseCase
        : UseCase<CreateTodoListInput, ICreateTodoListOutput, ITodoDbContextWrite>
    {
        public override async Task Execute()
        {
            Input.Validate();

            if (!Input.ValidationResult.IsValid)
            {
                Output.Invalid(Input.ValidationResult.Errors.First().ToString());
                return;
            }

            RequestResult requestResult = await Orchestrator.SendCommand(
                new CreateTodoListCommand
                {
                    Title = Input.Title
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }

        public CreateTodoListUseCase SetInput(CreateTodoListDto dto)
        {
            Input = new CreateTodoListInput(dto.Title);

            return this;
        }
    }
}