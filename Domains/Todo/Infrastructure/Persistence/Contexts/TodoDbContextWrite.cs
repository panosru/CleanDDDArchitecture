using CleanDDDArchitecture.Domains.Todo.Application.Persistence;
using Aviant.Infrastructure.Identity.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;

public sealed class TodoDbContextWrite
    : DbContextWrite<TodoDbContextWrite>, ITodoDbContextWrite
{
    #pragma warning disable 8618
    public TodoDbContextWrite(DbContextOptions<TodoDbContextWrite> options)
        : base(options)
    { }
    #pragma warning restore 8618

    #region ITodoDbContextWrite Members

    public DbSet<TodoListEntity> TodoLists { get; set; }

    public DbSet<TodoItemEntity> TodoItems { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Seed();

        base.OnModelCreating(modelBuilder);
    }
}
