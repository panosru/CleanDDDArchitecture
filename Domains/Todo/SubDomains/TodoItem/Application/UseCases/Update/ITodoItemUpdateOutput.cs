namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

using Aviant.Foundation.Application.UseCases;

public interface ITodoItemUpdateOutput : IUseCaseOutput
{
    public void Ok(object? @object);

    public void Invalid(string message);
}
