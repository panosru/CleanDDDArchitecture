namespace CleanDDDArchitecture.Application.Persistence
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using IApplicationDbContextBase = Aviant.DDD.Application.Persistance.IApplicationDbContext;

    public interface IApplicationDbContext : IApplicationDbContextBase
    {
        DbSet<TodoList> TodoLists { get; set; }

        DbSet<TodoItem> TodoItems { get; set; }
        
        DbSet<Account> Members { get; set; }
    }
}