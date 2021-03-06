#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public sealed class CreatedTodoListViewModel : IMapFrom<TodoListEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}