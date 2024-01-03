using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;

public interface IGetAllOutput : IUseCaseOutput
{
    public void Ok(object? @object);

    public void Invalid(string message);
}
