using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

public interface ITodoItemUpdateOutput : IUseCaseOutput
{
    public void Ok(object? @object);

    public void Invalid(string message);
}
