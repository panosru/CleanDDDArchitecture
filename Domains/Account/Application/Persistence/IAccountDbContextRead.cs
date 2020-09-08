namespace CleanDDDArchitecture.Domains.Account.Application.Persistence
{
    using Aggregates;
    using Aviant.DDD.Application.Persistance;
    using Microsoft.EntityFrameworkCore;

    public interface IAccountDbContextRead : IApplicationDbContextReadOnly
    {
        DbSet<AccountEntity> Accounts { get; set; }
    }
}