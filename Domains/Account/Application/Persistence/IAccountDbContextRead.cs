namespace CleanDDDArchitecture.Domains.Account.Application.Persistence
{
    #region

    using Aviant.DDD.Application.Persistance;
    using Core.Entities;
    using Microsoft.EntityFrameworkCore;

    #endregion

    public interface IAccountDbContextRead : IDbContextRead
    {
        DbSet<AccountEntity> Accounts { get; set; }
    }
}