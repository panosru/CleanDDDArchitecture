namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;

using Aviant.Foundation.Application.UseCases;

public sealed record TodoItemGetByInput(int Id) : UseCaseInput
{
    internal int Id { get; } = Id;
}
