namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete
{
    using Aviant.DDD.Application.UseCases;

    public class DeleteTodoListInput : IUseCaseInput
    {
        public DeleteTodoListInput(int id) => Id = id;

        public int Id { get; }
    }
}