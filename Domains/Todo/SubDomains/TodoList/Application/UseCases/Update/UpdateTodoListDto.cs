// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;

using System.ComponentModel.DataAnnotations;

public struct UpdateTodoListDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
}