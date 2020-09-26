namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class TodoItemGetByUseCase
        : UseCase<TodoItemGetByInput, ITodoItemGetByOutput>
    {
        public override async Task Execute(TodoItemGetByInput input)
        {
            RequestResult requestResult = await Orchestrator.SendQuery(
                new GetTodoItemQuery
                {
                    Id = input.Id
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}