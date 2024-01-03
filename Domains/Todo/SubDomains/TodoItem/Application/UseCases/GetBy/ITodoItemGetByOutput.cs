using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;

public interface ITodoItemGetByOutput : IUseCaseOutput
{
    public void Ok(object? @object);

    public void Invalid(string message);
}
