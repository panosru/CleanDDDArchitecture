namespace CleanDDDArchitecture.Infrastructure.Persistence.Contexts
{
    using Application.Persistence;
    using Aviant.DDD.Infrastructure.Persistance.Contexts;
    using Domain.Entities;
    using Identity;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class TodoDbContextReadOnly
        : ApplicationDbContextReadOnly<TodoUser, TodoRole>, ITodoDbContextReadOnly
    {
        public TodoDbContextReadOnly(
            DbContextOptions<TodoDbContextReadOnly> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }

        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TodoItemEntity> TodoItems { get; set; }

        public DbSet<AccountEntity> Accounts { get; set; }
    }
}