namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using Aviant.DDD.Application.UseCases;

    public class TodoItemDeleteInput : IUseCaseInput
    {
        public TodoItemDeleteInput(int id) => Id = id;

        public int Id { get; }
    }
}