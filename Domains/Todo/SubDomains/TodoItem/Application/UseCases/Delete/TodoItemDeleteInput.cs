using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete;

public sealed record TodoItemDeleteInput(int Id) : UseCaseInput
{
    internal int Id { get; } = Id;
}
