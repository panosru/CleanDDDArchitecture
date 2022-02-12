namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;

using Aviant.Foundation.Application.UseCases;
using Aviant.Foundation.Core.Configuration;

public sealed record TodoItemUpdateDetailsInput(
    int           Id,
    int           ListId,
    PriorityLevel Priority,
    string        Note) : UseCaseInput
{
    internal int Id { get; } = Id;

    internal int ListId { get; } = ListId;

    internal PriorityLevel Priority { get; } = Priority;

    internal string Note { get; } = Note;
}
