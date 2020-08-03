namespace CleanArchitecture.Application.TodoLists.Queries.ExportTodos
{
    using Aviant.DDD.Application.Mappings;
    using Domain.Entities;

    public class TodoItemRecord : IMapFrom<TodoItem>
    {
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}