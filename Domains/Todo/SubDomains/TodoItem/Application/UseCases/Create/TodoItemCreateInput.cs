using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;

public sealed record TodoItemCreateInput(int ListId, string Title) : UseCaseInput
{
    internal int ListId { get; } = ListId;

    internal string Title { get; } = Title;
}
