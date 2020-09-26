namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using Aviant.DDD.Application.UseCases;

    public class TodoItemDeleteInput : UseCaseInput
    {
        public TodoItemDeleteInput(int id) => Id = id;

        public int Id { get; }
    }
}