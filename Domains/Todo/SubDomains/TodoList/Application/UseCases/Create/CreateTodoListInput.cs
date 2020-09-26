namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public class CreateTodoListInput : UseCaseInput<CreateTodoListInput, CreateTodoListInputValidator>
    {
        public CreateTodoListInput(string title) => Title = title;

        public string Title { get; }

        public override void Validate() => UseDefaultValidation(this);
    }
}