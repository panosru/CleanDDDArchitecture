namespace CleanDDDArchitecture.Application.Persistence
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using IApplicationDbContextBase = Aviant.DDD.Application.Persistance.IApplicationDbContext;

    public interface IApplicationDbContext : IApplicationDbContextBase
    {
        DbSet<TodoListEntity> TodoLists { get; set; }

        DbSet<TodoItemEntity> TodoItems { get; set; }

        DbSet<AccountEntity> Members { get; set; } //TODO: Have a look 
    }
}