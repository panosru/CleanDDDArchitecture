namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

using Aviant.Application.UseCases;

public sealed record TodoItemUpdateInput(
    int    Id,
    string Title,
    bool   Done) : UseCaseInput
{
    internal int Id { get; } = Id;

    internal string Title { get; } = Title;

    internal bool Done { get; } = Done;
}
