// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

public struct CreateTodoListDto
{
    [Required]
    public string Title { get; set; }
}