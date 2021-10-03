// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System.ComponentModel.DataAnnotations;

    public struct CreateTodoListDto
    {
        [Required]
        public string Title { get; set; }
    }
}
