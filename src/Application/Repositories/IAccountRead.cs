namespace CleanDDDArchitecture.Application.Repositories
{
    using Aviant.DDD.Domain.Persistence;
    using Domain.Entities;

    public interface IAccountRead : IRepositoryRead<Account, int>
    {
        
    }
}