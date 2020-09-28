namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete
{
    using Aviant.DDD.Application.UseCases;

    public sealed class DeleteTodoListInput : UseCaseInput
    {
        public DeleteTodoListInput(int id) => Id = id;

        internal int Id { get; }
    }
}