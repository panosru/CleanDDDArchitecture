namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.UseCases;

    public sealed class CreateTodoListInput : UseCaseInput<CreateTodoListInput, CreateTodoListInputValidator>
    {
        public CreateTodoListInput(string title) => Title = title;

        internal string Title { get; }

        public override Task ValidateAsync(CancellationToken cancellationToken = default) =>
            UseDefaultValidation(this, cancellationToken);
    }
}