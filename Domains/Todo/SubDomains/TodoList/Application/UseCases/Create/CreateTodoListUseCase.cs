namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Todo.Application.Persistence;

    public sealed class CreateTodoListUseCase
        : UseCase<CreateTodoListInput, ICreateTodoListOutput, ITodoDbContextWrite>
    {
        public override async Task ExecuteAsync(
            CreateTodoListInput input,
            CancellationToken   cancellationToken = default)
        {
            await input.ValidateAsync(cancellationToken)
               .ConfigureAwait(false);

            if (!input.ValidationResult.IsValid)
            {
                Output.Invalid(input.ValidationResult.Errors.First().ToString());
                return;
            }

            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                    new CreateTodoListCommand(input.Title),
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}