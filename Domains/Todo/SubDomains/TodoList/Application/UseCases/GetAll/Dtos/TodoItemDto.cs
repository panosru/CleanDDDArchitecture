#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;

using System.Linq.Expressions;
using Todo.Core.Entities;

public sealed class TodoItemDto
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public string Title { get; set; }

    public bool Done { get; set; }

    public int Priority { get; set; }

    public string Note { get; set; }

    /// <summary>
    ///     Translated to SQL by EF Core when used inside a query projection.
    /// </summary>
    public static readonly Expression<Func<TodoItemEntity, TodoItemDto>> Projection = item => new TodoItemDto
    {
        Id       = item.Id,
        ListId   = item.ListId,
        Title    = item.Title,
        Done     = item.IsCompleted,
        Priority = (int)item.Priority,
        Note     = item.Note!
    };
}
