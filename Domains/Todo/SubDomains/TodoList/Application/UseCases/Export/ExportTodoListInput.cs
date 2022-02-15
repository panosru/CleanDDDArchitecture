// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

using Aviant.Application.UseCases;

public sealed record ExportTodoListInput(int ListId) : UseCaseInput
{
    internal int ListId { get; } = ListId;
}
