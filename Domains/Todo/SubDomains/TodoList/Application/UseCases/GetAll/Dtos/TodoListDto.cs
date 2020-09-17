namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos
{
    using System.Collections.Generic;
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public class TodoListDto : IMapFrom<TodoListEntity>
    {
        public TodoListDto() => Items = new List<TodoItemDto>();

        public int Id { get; set; }

        public string Title { get; set; }

        public IList<TodoItemDto> Items { get; set; }
    }
}