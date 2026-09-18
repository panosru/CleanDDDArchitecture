using CleanDDDArchitecture.Domains.Todo.Core.Entities;

#pragma warning disable 8618
namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

public sealed class CreatedTodoListViewModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public static CreatedTodoListViewModel From(TodoListEntity entity) => new()
    {
        Id    = entity.Id,
        Title = entity.Title
    };
}
