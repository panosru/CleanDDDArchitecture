namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete
{
    using Aviant.DDD.Application.UseCases;

    public class DeleteTodoListInput : UseCaseInput
    {
        public DeleteTodoListInput(int id) => Id = id;

        public int Id { get; }
    }
}