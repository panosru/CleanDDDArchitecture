// ReSharper disable UnusedAutoPropertyAccessor.Global

using Aviant.Application.Mappings;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

public sealed class TodoItemRecord : IMapFrom<TodoItemEntity>
{
    public string Title { get; set; }

    public bool Done { get; set; }
}
