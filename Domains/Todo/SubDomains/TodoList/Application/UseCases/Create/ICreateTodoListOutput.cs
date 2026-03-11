using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

public interface ICreateTodoListOutput : IUseCaseOutput
{
    public void Ok(object? payload);

    public void Invalid(string message);
}
