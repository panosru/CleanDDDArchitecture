// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

using Aviant.DDD.Application.UseCases;

public sealed class ExportTodoListInput : UseCaseInput
{
    public ExportTodoListInput(int listId) => ListId = listId;

    internal int ListId { get; }
}