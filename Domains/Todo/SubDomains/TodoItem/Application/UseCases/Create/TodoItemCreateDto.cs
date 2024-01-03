// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;

public struct TodoItemCreateDto
{
    [Required]
    public int ListId { get; set; }

    [Required]
    public string Title { get; set; }
}