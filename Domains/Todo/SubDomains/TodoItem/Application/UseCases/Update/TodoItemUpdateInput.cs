using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

public sealed record TodoItemUpdateInput(
    int    Id,
    string Title,
    bool   Done) : UseCaseInput
{
    internal int Id { get; } = Id;

    internal string Title { get; } = Title;

    internal bool Done { get; } = Done;
}
