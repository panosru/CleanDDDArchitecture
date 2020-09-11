namespace CleanDDDArchitecture.Domains.Account.Application.Persistence
{
    using Aviant.DDD.Application.Persistance;
    using Core.Entities;
    using Microsoft.EntityFrameworkCore;

    public interface IAccountDbContextWrite : IDbContextWrite
    {
        DbSet<AccountEntity> Accounts { get; set; }
    }
}