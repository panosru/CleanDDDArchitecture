namespace CleanDDDArchitecture.Application.Persistence
{
    using Accounts;
    using Aviant.DDD.Application.Persistance;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public interface ITodoDbContextReadOnly : IApplicationDbContextReadOnly
    {
        DbSet<TodoListEntity> TodoLists { get; set; }

        DbSet<TodoItemEntity> TodoItems { get; set; }

        // DbSet<AccountEntity> Accounts { get; set; }
    }
}