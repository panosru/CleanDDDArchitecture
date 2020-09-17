namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create.Dtos
{
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public class TodoListDto : IMapFrom<TodoListEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}