using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;

public sealed record TodoItemGetByInput(int Id) : UseCaseInput
{
    internal int Id { get; } = Id;
}
