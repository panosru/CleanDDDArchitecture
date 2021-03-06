namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using Aviant.DDD.Application.UseCases;

    public interface ICreateTodoListOutput : IUseCaseOutput
    {
        public void Ok(object? @object);

        public void Invalid(string message);
    }
}