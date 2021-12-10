#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;

using Aviant.DDD.Application.Mappings;
using Todo.Core.Entities;

public sealed class TodoItemViewModel : IMapFrom<TodoItemEntity>
{
    public int Id { get; set; }

    public string Title { get; set; }

    public int ListId { get; set; }
}