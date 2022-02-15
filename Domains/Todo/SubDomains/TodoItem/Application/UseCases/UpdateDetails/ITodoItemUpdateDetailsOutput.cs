namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;

using Aviant.Application.UseCases;

public interface ITodoItemUpdateDetailsOutput : IUseCaseOutput
{
    public void Invalid(string message);
}
