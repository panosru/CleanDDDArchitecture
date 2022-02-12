namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

using Aviant.Foundation.Application.UseCases;

public sealed record DeleteTodoListInput(int Id) : UseCaseInput
{
    internal int Id { get; } = Id;
}
