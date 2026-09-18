using CleanDDDArchitecture.Domains.Todo.Core.Entities;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

public sealed class TodoItemViewModel
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public string Title { get; set; }

    public bool IsCompleted { get; set; }

    public static TodoItemViewModel From(TodoItemEntity entity) => new()
    {
        Id          = entity.Id,
        ListId      = entity.ListId,
        Title       = entity.Title,
        IsCompleted = entity.IsCompleted
    };
}
