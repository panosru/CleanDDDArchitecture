namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts
{
    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Core.Entities;
    using Microsoft.EntityFrameworkCore;

    public sealed class TodoDbContextWrite
        : DbContextWrite<TodoDbContextWrite>, ITodoDbContextWrite
    {
        #pragma warning disable 8618
        public TodoDbContextWrite(DbContextOptions<TodoDbContextWrite> options)
            #pragma warning restore 8618
            : base(options)
        { }

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
}