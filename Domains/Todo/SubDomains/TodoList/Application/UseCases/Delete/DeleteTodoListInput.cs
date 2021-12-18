namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete;

using Aviant.DDD.Application.UseCases;

public sealed record DeleteTodoListInput(int Id) : UseCaseInput
{
    internal int Id { get; } = Id;
}
