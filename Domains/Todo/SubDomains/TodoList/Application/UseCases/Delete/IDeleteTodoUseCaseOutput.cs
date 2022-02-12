namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

using Aviant.Foundation.Application.UseCases;

public interface IDeleteTodoUseCaseOutput : IUseCaseOutput
{
    public void Invalid(string message);
}
