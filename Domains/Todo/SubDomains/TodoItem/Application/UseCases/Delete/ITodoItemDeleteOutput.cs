namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using Aviant.DDD.Application.UseCases;

    public interface ITodoItemDeleteOutput : IUseCaseOutput
    {
        public void Invalid(string message);
    }
}