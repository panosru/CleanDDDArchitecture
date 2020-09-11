namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts
{
    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Core.Entities;
    using Microsoft.EntityFrameworkCore;

    public class TodoDbContextRead
        : DbContextRead, ITodoDbContextRead
    {
        public TodoDbContextRead(DbContextOptions<TodoDbContextRead> options)
            : base(options)
        { }

        #region ITodoDbContextRead Members

        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TodoItemEntity> TodoItems { get; set; }

        #endregion
    }
}