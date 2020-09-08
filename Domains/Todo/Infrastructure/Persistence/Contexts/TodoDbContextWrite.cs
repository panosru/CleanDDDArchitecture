namespace CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts
{
    using Application.Persistence;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Infrastructure.Persistence.Contexts;
    using Core.Entities;
    using Identity;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class TodoDbContextWrite
        : ApplicationDbContext<TodoDbContextWrite, TodoUser, TodoRole>, ITodoDbContextWrite
    {
        public TodoDbContextWrite(
            DbContextOptions<TodoDbContextWrite> options,
            IOptions<OperationalStoreOptions>    operationalStoreOptions,
            ICurrentUserService                  currentUserService,
            IDateTimeService                     dateTimeService)
            : base(
                options,
                operationalStoreOptions,
                currentUserService,
                dateTimeService)
        { }

    #region ITodoDbContextWrite Members

        public DbSet<TodoListEntity> TodoLists { get; set; }

        public DbSet<TodoItemEntity> TodoItems { get; set; }

    #endregion
    }
}