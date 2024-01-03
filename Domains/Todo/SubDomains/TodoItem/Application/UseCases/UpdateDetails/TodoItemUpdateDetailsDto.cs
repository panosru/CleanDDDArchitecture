// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using Aviant.Core.Configuration;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;

public struct TodoItemUpdateDetailsDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int ListId { get; set; }

    [Required]
    public PriorityLevel Priority { get; set; }

    [Required]
    public string Note { get; set; }
}
