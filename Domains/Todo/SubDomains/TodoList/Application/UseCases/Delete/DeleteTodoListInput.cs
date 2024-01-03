using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

public sealed record DeleteTodoListInput(int Id) : UseCaseInput
{
    internal int Id { get; } = Id;
}
