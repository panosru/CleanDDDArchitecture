namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete;

using Aviant.Foundation.Application.UseCases;

public interface ITodoItemDeleteOutput : IUseCaseOutput
{
    public void Invalid(string message);
}
