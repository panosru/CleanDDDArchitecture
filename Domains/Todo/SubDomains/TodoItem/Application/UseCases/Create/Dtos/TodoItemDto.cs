namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create.Dtos
{
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public class TodoItemDto : IMapFrom<TodoItemEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ListId { get; set; }
    }
}