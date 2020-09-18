namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using Aviant.DDD.Application.UseCases;

    public interface IUpdateTodoListOutput : IUseCaseOutput
    {
        public void Invalid(string message);
    }
}