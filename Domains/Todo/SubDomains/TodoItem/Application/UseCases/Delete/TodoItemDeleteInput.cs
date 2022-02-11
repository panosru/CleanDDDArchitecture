namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete;

using Aviant.Application.UseCases;

public sealed record TodoItemDeleteInput(int Id) : UseCaseInput
{
    internal int Id { get; } = Id;
}
