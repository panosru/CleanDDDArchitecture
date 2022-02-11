// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;

using Aviant.Application.Mappings;
using Todo.Core.Entities;

public sealed class TodoListDto : IMapFrom<TodoListEntity>
{
    #pragma warning disable 8618
    public TodoListDto() => Items = new List<TodoItemDto>();
    #pragma warning restore 8618

    public int Id { get; set; }

    public string Title { get; set; }

    public IList<TodoItemDto> Items { get; set; }
}
