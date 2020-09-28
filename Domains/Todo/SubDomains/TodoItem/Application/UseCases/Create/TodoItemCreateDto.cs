// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using System.ComponentModel.DataAnnotations;

    public struct TodoItemCreateDto
    {
        [Required]
        public int ListId { get; set; }

        [Required]
        public string Title { get; set; }
    }
}