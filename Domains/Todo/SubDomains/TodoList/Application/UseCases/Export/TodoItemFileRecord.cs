namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public class TodoItemRecord : IMapFrom<TodoItemEntity>
    {
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}