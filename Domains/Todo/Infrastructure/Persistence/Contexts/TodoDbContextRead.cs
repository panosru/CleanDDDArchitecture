using CleanDDDArchitecture.Domains.Todo.Application.Persistence;
using Aviant.Infrastructure.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;

public sealed class TodoDbContextRead
    : DbContextRead, ITodoDbContextRead
{
    #pragma warning disable 8618
    public TodoDbContextRead(DbContextOptions<TodoDbContextRead> options)
        : base(options)
    { }
    #pragma warning restore 8618

    #region ITodoDbContextRead Members

    public DbSet<TodoListEntity> TodoLists { get; set; }

    public DbSet<TodoItemEntity> TodoItems { get; set; }

    #endregion
}
