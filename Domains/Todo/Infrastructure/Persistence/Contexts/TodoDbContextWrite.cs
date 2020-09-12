namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts
{
    #region

    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Core.Entities;
    using Microsoft.EntityFrameworkCore;

    #endregion

    public class TodoDbContextWrite
        : DbContextWrite<TodoDbContextWrite>, ITodoDbContextWrite
    {
        public TodoDbContextWrite(DbContextOptions<TodoDbContextWrite> options)
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