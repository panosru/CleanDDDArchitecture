using CleanDDDArchitecture.Domains.Todo.Core.Entities;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;

public sealed class TodoItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public int ListId { get; set; }

    public static TodoItemViewModel From(TodoItemEntity entity) => new()
    {
        Id     = entity.Id,
        Title  = entity.Title,
        ListId = entity.ListId
    };
}
