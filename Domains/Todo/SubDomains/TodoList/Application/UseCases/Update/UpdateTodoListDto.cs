// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;

public struct UpdateTodoListDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
}