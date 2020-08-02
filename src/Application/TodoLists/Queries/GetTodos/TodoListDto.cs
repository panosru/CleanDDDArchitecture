
namespace CleanArchitecture.Application.TodoLists.Queries.GetTodos
{
    using Domain.Entities;
    using System.Collections.Generic;
    using Aviant.DDD.Application.Mappings;

    public class TodoListDto : IMapFrom<TodoList>
{
    public TodoListDto()
    {
        Items = new List<TodoItemDto>();
    }

    public int Id { get; set; }

    public string Title { get; set; }

    public IList<TodoItemDto> Items { get; set; }
}
}
