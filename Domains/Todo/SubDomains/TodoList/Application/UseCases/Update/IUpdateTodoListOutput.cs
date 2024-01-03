using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;

public interface IUpdateTodoListOutput : IUseCaseOutput
{
    public void Invalid(string message);
}
