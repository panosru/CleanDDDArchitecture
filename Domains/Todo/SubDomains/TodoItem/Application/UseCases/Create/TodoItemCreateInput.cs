namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;

using Aviant.Foundation.Application.UseCases;

public sealed record TodoItemCreateInput(int ListId, string Title) : UseCaseInput
{
    internal int ListId { get; } = ListId;

    internal string Title { get; } = Title;
}
