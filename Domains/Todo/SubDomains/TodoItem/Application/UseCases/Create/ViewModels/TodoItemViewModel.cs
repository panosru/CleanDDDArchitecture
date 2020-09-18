namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create.ViewModels
{
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public class TodoItemViewModel : IMapFrom<TodoItemEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int ListId { get; set; }
    }
}