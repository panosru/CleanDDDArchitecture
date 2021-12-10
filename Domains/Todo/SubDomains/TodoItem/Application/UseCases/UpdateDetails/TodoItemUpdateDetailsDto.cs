// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;

using System.ComponentModel.DataAnnotations;
using Aviant.DDD.Core.Configuration;

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