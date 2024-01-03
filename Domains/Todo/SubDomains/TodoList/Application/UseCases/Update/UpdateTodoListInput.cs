using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;

public sealed record UpdateTodoListInput(int Id, string Title) : UseCaseInput
{
    internal int Id { get; } = Id;

    internal string Title { get; } = Title;
}
