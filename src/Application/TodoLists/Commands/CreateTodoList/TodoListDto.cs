namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    using Aviant.DDD.Application.Mappings;
    using Domain.Entities;

    public class TodoListDto : IMapFrom<TodoListEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}