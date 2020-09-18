namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public class CreateTodoListInput : IUseCaseInput
    {
        public CreateTodoListInput(string title) => Title = title;

        public string Title { get; }
    }
}