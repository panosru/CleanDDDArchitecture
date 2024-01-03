using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

public interface IDeleteTodoUseCaseOutput : IUseCaseOutput
{
    public void Invalid(string message);
}
