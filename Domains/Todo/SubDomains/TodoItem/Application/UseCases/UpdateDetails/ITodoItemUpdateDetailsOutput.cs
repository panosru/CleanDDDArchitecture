using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;

public interface ITodoItemUpdateDetailsOutput : IUseCaseOutput
{
    public void Invalid(string message);
}
