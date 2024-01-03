using Aviant.Application.Mappings;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

#pragma warning disable 8618
namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

public sealed class CreatedTodoListViewModel : IMapFrom<TodoListEntity>
{
    public int Id { get; set; }

    public string Title { get; set; }
}
