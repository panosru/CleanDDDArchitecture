namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;

using Application.Persistence;
using Aviant.Infrastructure.Persistence.Contexts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

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