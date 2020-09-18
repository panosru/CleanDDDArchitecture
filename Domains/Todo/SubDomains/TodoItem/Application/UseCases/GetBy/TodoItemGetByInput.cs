namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy
{
    using Aviant.DDD.Application.UseCases;

    public class TodoItemGetByInput : IUseCaseInput
    {
        public TodoItemGetByInput(int id) => Id = id;

        public int Id { get; }
    }
}