using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete;

public interface ITodoItemDeleteOutput : IUseCaseOutput
{
    public void Invalid(string message);
}
