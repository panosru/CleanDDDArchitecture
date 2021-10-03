namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public interface ITodoItemCreateOutput : IUseCaseOutput
    {
        public void Ok(object? @object);

        public void Invalid(string message);
    }
}
