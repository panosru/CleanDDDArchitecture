// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;

using System.Linq.Expressions;
using Todo.Core.Entities;

public sealed class TodoListDto
{
    #pragma warning disable 8618
    public TodoListDto() => Items = new List<TodoItemDto>();
    #pragma warning restore 8618

    public int Id { get; set; }

    public string Title { get; set; }

    public IList<TodoItemDto> Items { get; set; }

    /// <summary>
    ///     Translated to SQL by EF Core, items included, in a single query.
    /// </summary>
    public static readonly Expression<Func<TodoListEntity, TodoListDto>> Projection = list => new TodoListDto
    {
        Id    = list.Id,
        Title = list.Title,
        Items = list.Items.AsQueryable().Select(TodoItemDto.Projection).ToList()
    };
}
