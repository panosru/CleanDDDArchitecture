namespace CleanDDDArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    using Aviant.DDD.Application.Mappings;
    using Domain.Entities;

    public class TodoItemDto : IMapFrom<TodoItemEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ListId { get; set; }
    }
}