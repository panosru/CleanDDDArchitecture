namespace CleanDDDArchitecture.Application.Persistence
{
    using Aviant.DDD.Application.Persistance;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public interface ITodoDbContext : IApplicationDbContext
    {
        DbSet<TodoListEntity> TodoLists { get; set; }

        DbSet<TodoItemEntity> TodoItems { get; set; }

        // DbSet<AccountEntity> Accounts { get; set; }
    }
}