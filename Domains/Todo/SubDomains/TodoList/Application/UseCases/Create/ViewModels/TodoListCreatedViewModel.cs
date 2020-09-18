namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create.ViewModels
{
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public class TodoListCreatedViewModel : IMapFrom<TodoListEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}