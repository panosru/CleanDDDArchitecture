namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

using Aviant.DDD.Application.UseCases;

public interface IDeleteTodoUseCaseOutput : IUseCaseOutput
{
    public void Invalid(string message);
}