namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts
{
    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Core.Entities;
    using Identity;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class TodoDbContextRead
        : ApplicationDbContextReadOnly<TodoUser, TodoRole>, ITodoDbContextRead
    {
        public TodoDbContextRead(
            DbContextOptions<TodoDbContextRead> options,
            IOptions<OperationalStoreOptions>   operationalStoreOptions)
            : base(options, operationalStoreOptions)
        { }

    #region ITodoDbContextRead Members

        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TodoItemEntity> TodoItems { get; set; }

    #endregion

        // public DbSet<AccountEntity> Accounts { get; set; }
    }
}