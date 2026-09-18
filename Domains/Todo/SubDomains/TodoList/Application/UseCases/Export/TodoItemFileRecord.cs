// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Linq.Expressions;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export;

public sealed class TodoItemRecord
{
    public string Title { get; set; }

    public bool Done { get; set; }

    /// <summary>
    ///     Translated to SQL by EF Core, so only the two columns are read.
    /// </summary>
    public static readonly Expression<Func<TodoItemEntity, TodoItemRecord>> Projection = item => new TodoItemRecord
    {
        Title = item.Title,
        Done  = item.IsCompleted
    };
}
