namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy
{
    using Aviant.DDD.Application.UseCases;

    public sealed class TodoItemGetByInput : UseCaseInput
    {
        public TodoItemGetByInput(int id) => Id = id;

        internal int Id { get; }
    }
}