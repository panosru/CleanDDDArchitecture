// ReSharper disable MemberCanBeInternal

using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

public sealed record ExportTodoListInput(int ListId) : UseCaseInput
{
    internal int ListId { get; } = ListId;
}
