namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using Aviant.DDD.Application.UseCases;

    public sealed class TodoItemDeleteInput : UseCaseInput
    {
        public TodoItemDeleteInput(int id) => Id = id;

        internal int Id { get; }
    }
}