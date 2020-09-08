namespace CleanDDDArchitecture.Domains.Account.Application.Persistence
{
    using Aggregates;
    using Aviant.DDD.Application.Persistance;
    using Microsoft.EntityFrameworkCore;

    public interface IAccountDbContextWrite : IApplicationDbContext
    {
        DbSet<AccountEntity> Accounts { get; set; }
    }
}