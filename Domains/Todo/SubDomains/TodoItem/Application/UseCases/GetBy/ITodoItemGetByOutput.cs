namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;

using Aviant.Foundation.Application.UseCases;

public interface ITodoItemGetByOutput : IUseCaseOutput
{
    public void Ok(object? @object);

    public void Invalid(string message);
}
