namespace CleanDDDArchitecture.Application.Persistence
{
    using Aviant.DDD.Application.Persistance;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public interface IApplicationDbContextReadOnly : IApplicationDbContextReadOnlyBase
    {
        DbSet<TodoListEntity> TodoLists { get; set; }

        DbSet<TodoItemEntity> TodoItems { get; set; }

        DbSet<AccountEntity> Accounts { get; set; }
    }
}