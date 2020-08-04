namespace CleanDDDArchitecture.Application.Common.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    //using IApplicationDbContextBase = Aviant.DDD.Application.Persistance.IApplicationDbContext;

    public interface IApplicationDbContext // : IApplicationDbContextBase
    {
        DbSet<TodoList> TodoLists { get; set; }

        DbSet<TodoItem> TodoItems { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}